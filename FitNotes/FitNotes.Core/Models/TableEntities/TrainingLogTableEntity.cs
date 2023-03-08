using Azure;
using Azure.Data.Tables;

namespace FitNotes.Core.Models.TableEntities
{
    public record TrainingLogTableEntity : TrainingLog, ITableEntity
    {
        public string PartitionKey { get => Category; set => Category = value; }
        public string RowKey { get => Id.ToString(); set => Id = int.Parse(value); }
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
    }
}
