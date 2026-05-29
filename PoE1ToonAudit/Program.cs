using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer(); // apparently this for swagger

// Inject configuration
var config = builder.Configuration;
var usrAccountName = config["PathOfExile:AccountName"] ?? throw new Exception("Missing Account Name");
var usrToon = config["PathOfExile:ToonName"] ?? throw new Exception("Missing Character Name");
var usrPoeSessId = config["PathOfExile:SessionId"] // pull PoE:SessionId from local secrets
                      ?? throw new InvalidOperationException("POESESSID is missing from configuration");

//todo string validation in separate func

//Console.WriteLine($"Successfully loaded Session ID: {usrPoeSessId[..5]}..."); // test if SSID output works

var handler = new HttpClientHandler() { UseCookies = false };
using var client = new HttpClient(handler);

client.DefaultRequestHeaders.Add("User-Agent", "WebAPI/1.0.0 (contact: test@test.com) PoeLocalTest");
var payload = new FormUrlEncodedContent([ // send expected args
    new KeyValuePair<string, string>("accountName", usrAccountName),
    new KeyValuePair<string, string>("character", usrToon)
]);

// could try the below at some point i guess?
//var handler = new HttpClientHandler() {UseCookies = true, CookieContainer = new System.Net.CookieContainer()};

var request = new HttpRequestMessage(HttpMethod.Post, 
    "https://www.pathofexile.com/character-window/get-items"); // get poe character's items list with mods
request.Headers.Add("Cookie", $"POESESSID={usrPoeSessId}");
request.Content = payload;

var response = await client.SendAsync(request);
var rawJson = await response.Content.ReadAsStringAsync();

Console.WriteLine(rawJson.Substring(0, Math.Min(rawJson.Length, 1000))); // print first 1000 chars

var app = builder.Build();
app.MapControllers();
await app.RunAsync();

//var builder = WebApplication.CreateBuilder(args);
//
// Add services to the container.
//
//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//
//var app = builder.Build();
//
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}
//
//app.UseHttpsRedirection();
//
//app.UseAuthorization();
//
//app.MapControllers();
//
//app.Run();