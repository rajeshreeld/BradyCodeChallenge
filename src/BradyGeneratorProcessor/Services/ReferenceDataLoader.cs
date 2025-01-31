using System.Xml.Linq;
using BradyGeneratorProcessor.Models;

namespace BradyGeneratorProcessor.Services
{
    public class ReferenceDataLoader
    {
        public static ReferenceData LoadReferenceData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Reference data file not found: {filePath}");
                throw new FileNotFoundException("Reference data XML file is missing.", filePath);
            }

            XDocument doc = XDocument.Load(filePath);
            var factors = doc.Element("ReferenceData")?.Element("Factors");

            if (factors == null)
            {
                throw new Exception("Invalid ReferenceData.xml format.");
            }

            return new ReferenceData
            {
                ValueFactorHigh = double.Parse(factors.Element("ValueFactor")?.Element("High")?.Value ?? "0"),
                ValueFactorMedium = double.Parse(factors.Element("ValueFactor")?.Element("Medium")?.Value ?? "0"),
                ValueFactorLow = double.Parse(factors.Element("ValueFactor")?.Element("Low")?.Value ?? "0"),

                EmissionsFactorHigh = double.Parse(factors.Element("EmissionsFactor")?.Element("High")?.Value ?? "0"),
                EmissionsFactorMedium = double.Parse(factors.Element("EmissionsFactor")?.Element("Medium")?.Value ?? "0"),
                EmissionsFactorLow = double.Parse(factors.Element("EmissionsFactor")?.Element("Low")?.Value ?? "0"),
            };
        }
    }
}