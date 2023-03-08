using FitNotes.Core.Models.TableEntities;

namespace FitNotes.Core.Models
{
    public static class MappingExtensions
    {

        public static TrainingLogTableEntity ToTableEntity(this TrainingLog trainingLog) => new()
        {
            Id = trainingLog.Id,
            Category = trainingLog.Category,
            Comment = trainingLog.Comment,
            Date = DateTime.SpecifyKind(trainingLog.Date, DateTimeKind.Utc),
            Exercise = trainingLog.Exercise,
            IsPersonalRecord = trainingLog.IsPersonalRecord,
            MetricWeight = trainingLog.MetricWeight,
            Reps = trainingLog.Reps
        };


        public static ExerciseTableEntity ToTableEntity(this Exercise exercise) => new()
        {
            ExerciseName = exercise.ExerciseName,
            ExerciseCategory = exercise.ExerciseCategory
        };

        public static CategoryTableEntity ToTableEntity(this Category category) => new()
        {
            ExerciseCategory = category.ExerciseCategory
        };

    }
}
