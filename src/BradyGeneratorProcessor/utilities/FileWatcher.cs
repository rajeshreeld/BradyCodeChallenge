using System;
using System.IO;
using BradyGeneratorProcessor.Services;
using BradyGeneratorProcessor.Models;

namespace BradyGeneratorProcessor.Utilities
{
    public class FileWatcher
    {
        private readonly string _inputFolder;
        private readonly string _outputFolder;
        private readonly string _processedFolder;
        private readonly ReferenceData _referenceData;

        public FileWatcher(string inputFolder, string outputFolder, string processedFolder, string referenceDataPath)
        {
            _inputFolder = inputFolder;
            _outputFolder = outputFolder;
            _processedFolder = processedFolder;
            _referenceData = ReferenceDataLoader.LoadReferenceData(referenceDataPath);
            // Ensure the Processed folder exists
            if (!Directory.Exists(_processedFolder))
            {
                Directory.CreateDirectory(_processedFolder);
            }
        }

        public void StartWatching()
        {
            using var watcher = new FileSystemWatcher(_inputFolder);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Filter = "*.xml";
            watcher.Created += OnFileCreated;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder: {_inputFolder}");
            Console.WriteLine("Press 'q' to quit.");
            
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"New file detected: {e.FullPath}");
            var report = XmlParser.ParseXml(e.FullPath);
            if (report != null)
            {
                XmlParser.WriteOutputXml(report, _referenceData, e.FullPath, _outputFolder);
                MoveFileToProcessedFolder(e.FullPath);
            }
            else{
                Console.WriteLine($"Unable to process file {e.FullPath}");
            }
        }

        private void MoveFileToProcessedFolder(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                string processedFilePath = Path.Combine(_processedFolder, fileName);

                // Ensure no duplicate file exists
                if (File.Exists(processedFilePath))
                {
                    File.Delete(processedFilePath);
                }

                File.Move(filePath, processedFilePath);
                Console.WriteLine($"File moved to Processed folder: {processedFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving file to Processed folder: {ex.Message}");
            }
        }
    }
}