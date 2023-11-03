namespace FitNotes.Core.Models
{
    public class TrainingLogSession : ITrainingLogContainer
    {

        public DateOnly Date { get; set; }
        public List<TrainingLog> TrainingLogs { get; set; }

        public float GetTotalSessionVolume => TrainingLogs.Sum(tl => tl.Reps * tl.MetricWeight);
        public float GetSessionExerciseVolume(string exerciseName) => TrainingLogs
            .Where(tl => string.Equals(tl.Exercise, exerciseName))
            .Sum(tl => tl.Reps * tl.MetricWeight);

        public List<TrainingLog> GetTrainingLogs() => TrainingLogs;

        public List<TrainingLog> GetTrainingLogsForExercise(string exerciseName) => TrainingLogs
            .Where(tl => string.Equals(tl.Exercise, exerciseName))
            .ToList();

    }
}