namespace BradyGeneratorProcessor.Models
{
    public class GasGenerator
    {
        public string? Name { get; set; }
        public double EmissionsRating { get; set; }
        public List<GenerationDay> Generation { get; set; } = [];
    }
}