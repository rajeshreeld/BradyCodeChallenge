// See https://aka.ms/new-console-template for more information

using BradyGeneratorProcessor.Utilities;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        // Loading configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string inputPath = config["InputPath"] ?? "Input";
        string outputPath = config["OutputPath"] ?? "Output";
        string processedFilePath = config["ProcessedFilePath"] ?? "Input/Processed";
        string referenceDataPath = config["ReferenceDataPath"] ?? "Data/ReferenceData.xml";

        Console.WriteLine($"Input Folder: {inputPath}");
        Console.WriteLine($"Output Folder: {outputPath}");

        FileWatcher watcher = new(inputPath, outputPath, processedFilePath, referenceDataPath);
            watcher.StartWatching();

    }

   
}
