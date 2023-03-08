using Azure;
using Azure.Data.Tables;

namespace FitNotes.Core.Models.TableEntities
{
    public record CategoryTableEntity : Category, ITableEntity
    {
        public string PartitionKey { get => ExerciseCategory; set => ExerciseCategory = value; }
        public string RowKey { get => ExerciseCategory; set => ExerciseCategory = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
