using System.Text.Json.Serialization;

namespace HttpBin.Models;

public class CapturedRequest
{
    public Guid RequestId { get; set; }
    public Guid BucketId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public string? Body { get; set; } = "";

    public string Path { get; set; } = "/";
    public string Method { get; set; } = "GET";
    public Dictionary<string, string> QueryParameters { get; set; } =
        new Dictionary<string, string>();

    [JsonIgnore]
    public Bucket Bucket { get; set; }
}
