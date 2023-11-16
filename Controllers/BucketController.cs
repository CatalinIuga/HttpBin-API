using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HttpBin.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace HttpBin.Controllers
{
    [ApiController]
    [Route("api/buckets")]
    [SwaggerTag("Create, read and delete request buckets.")]
    public class BucketController : ControllerBase
    {
        private readonly HttpBinDbContext _context;

        public BucketController(HttpBinDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        [Route("/all")]
        public async Task<ActionResult<Bucket>> GetBuckets()
        {
            var buckets = await _context.Buckets.ToListAsync();
            return Ok(buckets);
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Creates a new bucket and a JWT token.",
            Description = "On a bucket creation request, we also send a JWT Token with the bucketId as payload for further usage."
        )]
        public async Task<ActionResult<Bucket>> CreateBucket()
        {
            var bucket = new Bucket
            {
                UpdatedAt = DateTime.UtcNow,
                RequestCount = 0,
                TimeToLive = 1800
            };

            _context.Buckets.Add(bucket);
            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new Claim("bucketId", bucket.BucketId.ToString()) };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7134",
                audience: "https://localhost:5173/",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            var cookieOptions = new CookieOptions()
            {
                Domain = "localhost",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("JWT", tokenString, cookieOptions);
            return CreatedAtAction(nameof(GetBucket), new { bucketId = bucket.BucketId }, bucket);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Returns the bucket for the recieved JWT token.",
            Description = "This will be the checking endpoint for the frontend."
        )]
        public async Task<ActionResult<Bucket>> GetBucket()
        {
            var token = Request.Cookies["JWT"];
            if (token == null)
                return Unauthorized(new { message = "Create a bucket first!" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (tokenRead == null)
                return Unauthorized(new { message = "Invalid JWT token!" });

            var bucketIdClaim = tokenRead.Claims.FirstOrDefault(c => c.Type == "bucketId")?.Value;
            if (bucketIdClaim == null)
                return Unauthorized(new { message = "No bucketId found." });

            var bucket = await _context.Buckets.FindAsync(Guid.Parse(bucketIdClaim));
            if (bucket == null)
                return NotFound(new { message = "Bucket not found. Try creating a new one." });

            return Ok(bucket);
        }

        [HttpPatch]
        [Route("{bucketid}")]
        [SwaggerOperation(Summary = "Extends a bucket time to live.")]
        public async Task<ActionResult<Bucket>> UpdateBucket(Guid bucketId)
        {
            var bucket = _context.Buckets.Find(bucketId);
            if (bucket == null)
            {
                return NotFound();
            }
            if (bucket.TimeToLive > 24 * 3600)
                return BadRequest(
                    new
                    {
                        message = "The bucket has a 24h max time to live. Please make a new bucket."
                    }
                );

            bucket.TimeToLive += 1800;
            bucket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(bucket);
        }

        [HttpDelete]
        [Route("{bucketId}")]
        [SwaggerOperation(Summary = "Deletes the bucket with the specified Id.")]
        public async Task<ActionResult> DeleteBucket(Guid bucketId)
        {
            var bucket = await _context.Buckets.FindAsync(bucketId);
            if (bucket == null)
                return NotFound();

            _context.Buckets.Remove(bucket);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
