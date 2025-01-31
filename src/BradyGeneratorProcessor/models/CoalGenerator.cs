namespace BradyGeneratorProcessor.Models
{
    public class CoalGenerator
    {
        public string? Name { get; set; }
        public double TotalHeatInput { get; set; }
        public double ActualNetGeneration { get; set; }
        public double EmissionsRating { get; set; }
        public List<GenerationDay> Generation { get; set; } = new();
    }
}