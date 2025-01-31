namespace BradyGeneratorProcessor.Models
{
    public class GenerationReport
    {
        public List<WindGenerator> WindGenerators { get; set; } = [];
        public List<GasGenerator> GasGenerators { get; set; } = [];
        public List<CoalGenerator> CoalGenerators { get; set; } = [];
    }
}