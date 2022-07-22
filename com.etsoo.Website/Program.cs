using AspNetCoreRateLimit;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Serilog
// Install Serilog
// Install Serilog.AspNetCore
// Install Serilog.Sinks.File
builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Add services to the container.
var services = builder.Services;
services.AddRazorPages();

// Health check
// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
services.AddHealthChecks();

// IP rate limit
services.AddOptions();
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
services.AddInMemoryRateLimiting();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

services.AddControllers().AddJsonOptions(configure =>
{
    // Change the Json options here
    var options = configure.JsonSerializerOptions;

    options.WriteIndented = false;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.PropertyNameCaseInsensitive = true;
    options.DictionaryKeyPolicy = options.PropertyNamingPolicy;
});

var app = builder.Build();

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapHealthChecks("/hc");

app.Run();
