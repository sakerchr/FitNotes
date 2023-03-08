namespace FitNotes.Core.Models
{
    public record TrainingLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Exercise { get; set; }
        public string Category { get; set; }
        public float MetricWeight { get; set; }
        public int Reps { get; set; }
        public bool IsPersonalRecord { get; set; }
        public string Comment { get; set; }
    }
}
