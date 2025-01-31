using System;
using System.IO;
using NSubstitute;
using Xunit;
using BradyGeneratorProcessor.Utilities;
using BradyGeneratorProcessor.Services;
using BradyGeneratorProcessor.Models;

namespace BradyGeneratorProcessor.Tests.Utilities
{
    public class FileWatcherTests
    {
         private string GetReferenceDataPath()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "ReferenceData.xml");
            Console.WriteLine($"Reference Data Path: {path}");
            return path;
        }

        [Fact]
        public void MoveFileToProcessedFolder_ShouldMoveFile()
        {
            // Arrange
            string inputFolder = Path.GetTempPath();
            string processedFolder = Path.Combine(inputFolder, "Processed");
            string referenceDataPath = GetReferenceDataPath();

            Assert.True(File.Exists(referenceDataPath), $"ReferenceData.xml not found at: {referenceDataPath}");

            string tempFile = Path.Combine(inputFolder, "testfile.xml");
            Directory.CreateDirectory(processedFolder);
            File.WriteAllText(tempFile, "<xml></xml>");

            var watcher = new FileWatcher(inputFolder, inputFolder, processedFolder, referenceDataPath);

            // Act
            watcher.GetType().GetMethod("MoveFileToProcessedFolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                   ?.Invoke(watcher, new object[] { tempFile });

            // Assert
            string processedFilePath = Path.Combine(processedFolder, "testfile.xml");
            Assert.False(File.Exists(tempFile)); // File should be moved
            Assert.True(File.Exists(processedFilePath)); // File should exist in processed folder

            // Cleanup
            File.Delete(processedFilePath);
        }

        [Fact]
        public void OnFileCreated_ShouldProcessXmlFile()
        {
            // Arrange
            string inputFolder = Path.GetTempPath();
            string outputFolder = Path.GetTempPath();
            string processedFolder = Path.Combine(inputFolder, "Processed");
            string referenceDataPath = GetReferenceDataPath();
            string tempFile = Path.Combine(inputFolder, "testfile.xml");

            Assert.True(File.Exists(referenceDataPath), $"Test data file not found: {referenceDataPath}");

            File.WriteAllText(tempFile, "<GenerationReport></GenerationReport>");

            var watcher = new FileWatcher(inputFolder, outputFolder, processedFolder, referenceDataPath);
            var method = watcher.GetType().GetMethod("OnFileCreated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            method?.Invoke(watcher, new object[] { null, new FileSystemEventArgs(WatcherChangeTypes.Created, inputFolder, "testfile.xml") });

            // Assert
            string outputFilePath = Path.Combine(outputFolder, "testfile_result.xml");
            Assert.True(File.Exists(outputFilePath), "Output XML should be generated");

            // Cleanup
            File.Delete(outputFilePath);
            File.Delete(tempFile);
        }
    }
}