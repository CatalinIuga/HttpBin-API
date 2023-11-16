using Microsoft.AspNetCore.Mvc;
using HttpBin.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace HttpBin.Controllers;

[ApiController]
[Route("{bucketId}")]
[SwaggerTag("Receives and replays the requests for the specified bucket.")]
public class RequestReceiverController : ControllerBase
{
    private readonly HttpBinDbContext _context;

    public RequestReceiverController(HttpBinDbContext dbContext)
    {
        _context = dbContext;
    }

    // BASE ENDPOINT
    [HttpGet, HttpHead, HttpPost, HttpPut, HttpOptions, HttpPatch, HttpDelete]
    [SwaggerOperation(
        Summary = "Catches request for the specified HTTP method, bucket Id, and optionally the path."
    )]
    public async Task<ActionResult> ReceiveWebhook(Guid bucketId)
    {
        var bucket = _context.Buckets.Find(bucketId);
        if (bucket == null)
        {
            return NotFound(new { error = "Bucket not found." });
        }

        using var reader = new StreamReader(Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        var method = Request.Method;

        var headers = Request.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));

        var queryParameters = Request.Query.ToDictionary(
            q => q.Key,
            q => string.Join(";", q.Value)
        );

        var request = new CapturedRequest
        {
            BucketId = bucketId,
            Bucket = bucket,
            Headers = headers,
            Body = requestBody,
            Method = method,
            QueryParameters = queryParameters,
            Path = "/"
        };

        _context.Requests.Add(request);
        bucket.RequestCount++;
        _context.SaveChanges();

        return Ok(new { request });
    }

    // CATCHES REQUEST WITH MORE PATHS
    [HttpGet, HttpHead, HttpPost, HttpPut, HttpOptions, HttpPatch, HttpDelete]
    [Route("{*path}")]
    [SwaggerOperation(
        Summary = "Catches request for the specified HTTP method, bucket Id, and optionally the path."
    )]
    public async Task<ActionResult> ReceiveWebhookWithPath(Guid bucketId, [FromRoute] string path)
    {
        Console.WriteLine(path);
        var bucket = _context.Buckets.Find(bucketId);
        if (bucket == null)
        {
            return NotFound(new { error = "Bucket not found." });
        }

        using var reader = new StreamReader(Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        var method = Request.Method;

        var headers = Request.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));

        var queryParameters = Request.Query.ToDictionary(
            q => q.Key,
            q => string.Join(";", q.Value)
        );

        var request = new CapturedRequest
        {
            BucketId = bucketId,
            Bucket = bucket,
            Headers = headers,
            Body = requestBody,
            ReceivedAt = DateTime.UtcNow,
            Method = method,
            QueryParameters = queryParameters,
            Path = "/" + path
        };

        _context.Requests.Add(request);
        bucket.RequestCount++;
        _context.SaveChanges();

        return Ok(new { request });
    }
}
