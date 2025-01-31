using System;
using System.IO;
using Xunit;
using BradyGeneratorProcessor.Models;
using BradyGeneratorProcessor.Services;

namespace BradyGeneratorProcessor.Tests.Services
{
    public class ReferenceDataLoaderTests
    {

        private string GetTestFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "ReferenceData.xml");
        }

        [Fact]
        public void LoadReferenceData_ShouldReturnCorrectValues()
        {
            // Arrange
            string testFilePath = GetTestFilePath();
            Assert.True(File.Exists(testFilePath), $"Test data file not found: {testFilePath}");

            // Act
            ReferenceData referenceData = ReferenceDataLoader.LoadReferenceData(testFilePath);

            // Assert
            Assert.Equal(0.946, referenceData.ValueFactorHigh);
            Assert.Equal(0.696, referenceData.ValueFactorMedium);
            Assert.Equal(0.265, referenceData.ValueFactorLow);
        }
    }
    
}