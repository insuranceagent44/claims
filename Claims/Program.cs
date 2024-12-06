using System.Text.Json.Serialization;
using Claims.Audit;
using Claims.Audit.Models;
using Claims.Models;
using Claims.PremiumCalculator;
using Claims.Services;
using Claims.Validators;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        
    })
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ClaimsContext>(
    options =>
    {
        var client = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

builder.Services.AddAudit();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();
builder.Services.AddScoped<IPremiumCalculator, PremiumCalculator>();
builder.Services.AddScoped<ClaimValidator>();
builder.Services.AddScoped<CoverValidator>();
builder.Services.AddScoped<IPremiumTypeFactory, PremiumTypeFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

namespace Claims
{
    public partial class Program { }
}
