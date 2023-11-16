using Microsoft.AspNetCore.Mvc;
using HttpBin.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace HttpBin.Controllers
{
    [ApiController]
    [Route("api/requests")]
    [SwaggerTag("View and delete request from buckets.")]
    public class RequestController : ControllerBase
    {
        private readonly HttpBinDbContext _context;

        public RequestController(HttpBinDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("{requestId}")]
        [SwaggerOperation(Summary = "Returns the request with the specified Id.")]
        public ActionResult<CapturedRequest> GetRequest(Guid requestId)
        {
            var request = _context.Requests.Find(requestId);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpGet]
        [Route("bucket/{bucketId}")]
        [SwaggerOperation(Summary = "Returns the requests of specified bucket Id.")]
        public ActionResult<IEnumerable<CapturedRequest>> GetRequestsByBucket(Guid bucketId)
        {
            var requests = _context.Requests
                .Where(r => r.BucketId == bucketId)
                .Select(
                    r =>
                        new RequestInfoDto
                        {
                            RequestId = r.RequestId,
                            ReceivedAt = r.ReceivedAt,
                            Method = r.Method,
                            Path = r.Path
                        }
                )
                .ToList();

            return Ok(requests);
        }

        [HttpDelete("{requestId}")]
        [SwaggerOperation(Summary = "Deletes the request with the specified Id.")]
        public ActionResult DeleteRequest(Guid requestId)
        {
            var request = _context.Requests.Find(requestId);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

public class RequestInfoDto
{
    public Guid RequestId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
}
