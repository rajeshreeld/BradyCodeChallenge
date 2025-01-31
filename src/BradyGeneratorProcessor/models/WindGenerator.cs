namespace BradyGeneratorProcessor.Models
{
    public class WindGenerator
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public List<GenerationDay> Generation { get; set; } = [];
    }
}