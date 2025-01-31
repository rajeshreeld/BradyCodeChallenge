namespace BradyGeneratorProcessor.Models.Output
{
    public class GenerationOutput
    {
        public List<GeneratorTotal> Totals { get; set; } = new();
        public List<MaxEmissionDay> MaxEmissionGenerators { get; set; } = new();
        public List<ActualHeatRate> ActualHeatRates { get; set; } = new();
    }
}