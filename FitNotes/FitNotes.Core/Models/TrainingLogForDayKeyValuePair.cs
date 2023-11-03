namespace FitNotes.Core.Models
{
    public record TrainingLogForDayKeyValuePair(DateOnly Date, List<TrainingLog> TrainingLogs) : ITrainingLogContainer
    {
        public List<TrainingLog> GetTrainingLogs() => TrainingLogs;
    }
}