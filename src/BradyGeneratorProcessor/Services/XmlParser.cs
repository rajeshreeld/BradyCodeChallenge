using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using BradyGeneratorProcessor.Models;
using BradyGeneratorProcessor.Models.Output;

namespace BradyGeneratorProcessor.Services
{
    public class XmlParser
    {
        public static GenerationReport? ParseXml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return null;
            }

            XDocument doc = XDocument.Load(filePath);

            if (!ValidateXml(doc))
            {
                Console.WriteLine("XML validation failed. Skipping file: " + filePath);
                return null;
            }
            
            GenerationReport report = new()
            {
                // Parse Wind Generators
                WindGenerators = doc.Descendants("WindGenerator")
                    .Select(static w => new WindGenerator
                    {
                        Name = w.Element("Name")?.Value ?? "Unknown",
                        Location = w.Element("Location")?.Value ?? "Unknown",
                        Generation = (w.Element("Generation")?.Elements("Day") ?? Enumerable.Empty<XElement>())
                        .Select(static d => new GenerationDay
                        {
                            Date = DateTime.TryParse(d.Element("Date")?.Value, out DateTime date) ? date : DateTime.MinValue,
                            Energy = double.TryParse(d.Element("Energy")?.Value, out double energy) ? energy : 0,
                            Price = double.TryParse(d.Element("Price")?.Value, out double price) ? price : 0
                        }).ToList()
                    }).ToList(),

                // Parse Gas Generators
                GasGenerators = doc.Descendants("GasGenerator")
                    .Select(static g => new GasGenerator
                    {
                        Name = g.Element("Name")?.Value ?? "Unknown",
                        EmissionsRating = double.TryParse(g.Element("EmissionsRating")?.Value, out double rating) ? rating : 0,
                        Generation = (g.Element("Generation")?.Elements("Day") ?? Enumerable.Empty<XElement>())
                        .Select(static d => new GenerationDay
                        {
                            Date = DateTime.TryParse(d.Element("Date")?.Value, out DateTime date) ? date : DateTime.MinValue,
                            Energy = double.TryParse(d.Element("Energy")?.Value, out double energy) ? energy : 0,
                            Price = double.TryParse(d.Element("Price")?.Value, out double price) ? price : 0
                        }).ToList()
                    }).ToList(),

                // Parse Coal Generators
                CoalGenerators = doc.Descendants("CoalGenerator")
                    .Select(static c => new CoalGenerator
                    {
                        Name = c.Element("Name")?.Value ?? "Unknown",
                        TotalHeatInput = double.TryParse(c.Element("TotalHeatInput")?.Value, out double heatInput) ? heatInput : 0,
                        ActualNetGeneration = double.TryParse(c.Element("ActualNetGeneration")?.Value, out double netGen) ? netGen : 0,
                        EmissionsRating = double.TryParse(c.Element("EmissionsRating")?.Value, out double rating) ? rating : 0,
                        Generation = (c.Element("Generation")?.Elements("Day") ?? Enumerable.Empty<XElement>()).Select(static d => new GenerationDay
                        {
                            Date = DateTime.TryParse(d.Element("Date")?.Value, out DateTime date) ? date : DateTime.MinValue,
                            Energy = double.TryParse(d.Element("Energy")?.Value, out double energy) ? energy : 0,
                            Price = double.TryParse(d.Element("Price")?.Value, out double price) ? price : 0
                        }).ToList()
                    }).ToList()
            };

            return report;
        }

        public static void WriteOutputXml(GenerationReport report, ReferenceData referenceData, string inputFilePath, string outputFolder)
        {
            string inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
            string outputFilePath = Path.Combine(outputFolder, $"{inputFileName}_result.xml");


            // Compute Totals with Value Factor
            List<GeneratorTotal> totals = new();

            
            foreach (var generator in report.WindGenerators)
            {
                double valueFactor = generator.Location == "Onshore"
                    ? referenceData.ValueFactorHigh
                    : referenceData.ValueFactorLow;

                totals.Add(new GeneratorTotal
                {
                    Name = generator.Name,
                    Total = generator.Generation.Sum(d => d.Energy * d.Price * valueFactor)
                });
            }

            totals.AddRange(report.GasGenerators.Select(g => new GeneratorTotal
            {
                Name = g.Name,
                Total = g.Generation.Sum(d => d.Energy * d.Price * referenceData.ValueFactorMedium) // Value factor for gas is "Medium"
            }));

            totals.AddRange(report.CoalGenerators.Select(g => new GeneratorTotal
            {
                Name = g.Name,
                Total = g.Generation.Sum(d => d.Energy * d.Price * referenceData.ValueFactorMedium) // Value factor for coal is "Medium"
            }));

            // Compute Max Emission Per Day using Emission Factor
            List<MaxEmissionDay> maxEmissionGenerators = new();
            var allDays = report.GasGenerators.SelectMany(g => g.Generation)
                        .Concat(report.CoalGenerators.SelectMany(c => c.Generation))
                        .Select(d => d.Date)
                        .Distinct();

            foreach (var date in allDays)
            {
                var emissions = new List<(string? Name, double Emission)>();

                foreach (var generator in report.GasGenerators)
                {
                    var day = generator.Generation.FirstOrDefault(d => d.Date == date);
                    if (day != null)
                    {
                        double factor = referenceData.EmissionsFactorMedium; // Emission factor for gas is "Medium"
                        emissions.Add((generator.Name, day.Energy * generator.EmissionsRating * factor));
                    }
                }

                foreach (var generator in report.CoalGenerators)
                {
                    var day = generator.Generation.FirstOrDefault(d => d.Date == date);
                    if (day != null)
                    {
                        double factor = referenceData.EmissionsFactorHigh; // Emission factor for coal is "High"
                        emissions.Add((generator.Name, day.Energy * generator.EmissionsRating * factor));
                    }
                }

                if (emissions.Any())
                {
                    var maxEmission = emissions.OrderByDescending(e => e.Emission).First();
                    maxEmissionGenerators.Add(new MaxEmissionDay
                    {
                        Name = maxEmission.Name,
                        Date = date,
                        Emission = maxEmission.Emission
                    });
                }
            }

            // Create Output XML
            XDocument outputDoc = new(
                new XElement("GenerationOutput",
                    new XElement("Totals",
                        totals.Select(t => new XElement("Generator",
                            new XElement("Name", t.Name),
                            new XElement("Total", t.Total)
                        ))
                    ),
                    new XElement("MaxEmissionGenerators",
                        maxEmissionGenerators.Select(e => new XElement("Day",
                            new XElement("Name", e.Name),
                            new XElement("Date", e.Date.ToString("yyyy-MM-ddTHH:mm:sszzz")),
                            new XElement("Emission", e.Emission)
                        ))
                    )
                )
            );

            outputDoc.Save(outputFilePath);
            Console.WriteLine($"Output XML saved: {outputFilePath}");
        }

        private static bool ValidateXml(XDocument doc)
        {
            bool isValid = true;
            
            // Validate Wind Generators
            foreach (var windGenerator in doc.Descendants("WindGenerator"))
            {
                if (windGenerator.Element("Name") == null || string.IsNullOrWhiteSpace(windGenerator.Element("Name")?.Value))
                {
                    Console.WriteLine("Validation Error: Missing <Name> in WindGenerator.");
                    isValid = false;
                }

                if (windGenerator.Element("Generation") == null)
                {
                    Console.WriteLine("Validation Error: Missing <Generation> in WindGenerator.");
                    isValid = false;
                }
            }

            // Validate Gas Generators
            foreach (var gasGenerator in doc.Descendants("GasGenerator"))
            {
                if (gasGenerator.Element("Name") == null || string.IsNullOrWhiteSpace(gasGenerator.Element("Name")?.Value))
                {
                    Console.WriteLine("Validation Error: Missing <Name> in GasGenerator.");
                    isValid = false;
                }

                if (gasGenerator.Element("EmissionsRating") == null ||
                    !double.TryParse(gasGenerator.Element("EmissionsRating")?.Value, out _))
                {
                    Console.WriteLine("Validation Error: Invalid or missing <EmissionsRating> in GasGenerator.");
                    isValid = false;
                }
            }

            // Validate Coal Generators
            foreach (var coalGenerator in doc.Descendants("CoalGenerator"))
            {
                if (coalGenerator.Element("Name") == null || string.IsNullOrWhiteSpace(coalGenerator.Element("Name")?.Value))
                {
                    Console.WriteLine("Validation Error: Missing <Name> in CoalGenerator.");
                    isValid = false;
                }

                if (coalGenerator.Element("TotalHeatInput") == null ||
                    !double.TryParse(coalGenerator.Element("TotalHeatInput")?.Value, out _))
                {
                    Console.WriteLine("Validation Error: Invalid or missing <TotalHeatInput> in CoalGenerator.");
                    isValid = false;
                }

                if (coalGenerator.Element("ActualNetGeneration") == null ||
                    !double.TryParse(coalGenerator.Element("ActualNetGeneration")?.Value, out _))
                {
                    Console.WriteLine("Validation Error: Invalid or missing <ActualNetGeneration> in CoalGenerator.");
                    isValid = false;
                }
            }

            return isValid;
        }
    
    }
}