using Azure;
using Azure.Data.Tables;

namespace FitNotes.Core.Models.TableEntities
{
    public record ExerciseTableEntity : Exercise, ITableEntity
    {
        public string PartitionKey { get => ExerciseCategory; set => ExerciseCategory = value; }
        public string RowKey { get => ExerciseName; set => ExerciseName = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
