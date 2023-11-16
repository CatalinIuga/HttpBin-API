using Microsoft.EntityFrameworkCore;

namespace HttpBin.Models;

public class HttpBinDbContext : DbContext
{
    public DbSet<Bucket> Buckets { get; set; }
    public DbSet<CapturedRequest> Requests { get; set; }

    public HttpBinDbContext(DbContextOptions<HttpBinDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bucket>().HasKey(b => b.BucketId);
        modelBuilder.Entity<CapturedRequest>().HasKey(r => r.RequestId);
        modelBuilder
            .Entity<CapturedRequest>()
            .HasOne(r => r.Bucket)
            .WithMany(b => b.Requests)
            .HasForeignKey(r => r.BucketId);
    }
}
