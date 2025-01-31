using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using BradyGeneratorProcessor.Models;
using BradyGeneratorProcessor.Models.Output;
using BradyGeneratorProcessor.Services;

namespace BradyGeneratorProcessor.Tests.Services
{
    public class XmlParserTests
    {
        [Fact]
        public void WriteOutputXml_ShouldGenerateValidXml()
        {
            // Arrange
            string tempInputFile = Path.GetTempFileName();
            string outputFolder = Path.GetTempPath();

            var referenceData = new ReferenceData
            {
                ValueFactorHigh = 0.946,
                ValueFactorMedium = 0.696,
                ValueFactorLow = 0.265,
                EmissionsFactorHigh = 0.812,
                EmissionsFactorMedium = 0.562,
                EmissionsFactorLow = 0.312
            };

            var report = new GenerationReport
            {
                WindGenerators = new List<WindGenerator>
                {
                    new WindGenerator
                    {
                        Name = "Wind[Offshore]",
                        Location = "Offshore",
                        Generation = new List<GenerationDay>
                        {
                            new GenerationDay { Date = DateTime.Parse("2023-01-01"), Energy = 100, Price = 20 }
                        }
                    }
                },
                GasGenerators = new List<GasGenerator>(),
                CoalGenerators = new List<CoalGenerator>()
            };

            // Act
            XmlParser.WriteOutputXml(report, referenceData, tempInputFile, outputFolder);

            // Assert
            string outputFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(tempInputFile) + "_result.xml");
            Assert.True(File.Exists(outputFilePath));

            // Cleanup
            File.Delete(outputFilePath);
            File.Delete(tempInputFile);
        }
    }
}