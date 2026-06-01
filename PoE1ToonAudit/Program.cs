var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<PoE1ToonAudit.Dao.PoeApiDao>();
builder.Services.AddScoped<PoE1ToonAudit.Services.ToonAuditService>();
// builder.Services.AddEndpointsApiExplorer(); // for potential extension with swagger

// Inject configuration - moved to Dao
// var config = builder.Configuration;

var app = builder.Build();
app.MapControllers();
await app.RunAsync();