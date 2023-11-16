using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace HttpBin.Models;

public class Bucket
{
    public Guid BucketId { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public int RequestCount { get; set; }
    public long TimeToLive { get; set; }

    [JsonIgnore]
    public ICollection<CapturedRequest> Requests { get; set; } = new Collection<CapturedRequest>();
}
