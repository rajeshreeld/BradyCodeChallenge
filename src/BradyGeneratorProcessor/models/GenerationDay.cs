using System;

namespace BradyGeneratorProcessor.Models
{
    public class GenerationDay
    {
        public DateTime Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
    }
}