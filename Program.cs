using HttpBin.Models;
using HttpBin.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HttpBinDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
);
builder.Services.AddHostedService<BucketExpirationBackgroundService>(); // deletes expired buckets

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "HttpBin",
            Description = "Capture and inspect HTTP Requests.",
            Version = "v1"
        }
    );
    options.EnableAnnotations();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddCors(
    options =>
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:5173");
            policy.AllowCredentials();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
