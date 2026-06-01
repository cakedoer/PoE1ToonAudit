using System.Text.Json;
using PoE1ToonAudit.Models;

namespace PoE1ToonAudit.Dao;

public class PoeApiDao(IConfiguration config, ILogger<PoeApiDao> logger)
{
    public async Task<PoeToon> FetchCharacterItemsAsync()
    {   
        // appsettings.json for account name and character name, local secrets for poesessid

        var accountName = config["PathOfExile:AccountName"] ?? throw new Exception("Missing account name");
        var toonName    = config["PathOfExile:ToonName"]    ?? throw new Exception("Missing character name");
        var sessId = config["PathOfExile:SessionId"]   ?? throw new InvalidOperationException("POESESSID is missing from configuration");

        // i hate var i regret everything
        // todo string validation for the above in separate method
        
        var handler = new HttpClientHandler() { UseCookies = false };
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("User-Agent", "WebAPI/1.0.0 (contact: test@test.com) PoeLocalTest");
        
        //Console.WriteLine($"Successfully loaded Session ID: {usrPoeSessId[..5]}..."); // test if SSID output works

        // get poe character's items list with mods
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://www.pathofexile.com/character-window/get-items");
        
        request.Headers.Add("Cookie", $"POESESSID={sessId}");
        request.Content = new FormUrlEncodedContent
        ([
            new KeyValuePair<string, string>("accountName", accountName),
            new KeyValuePair<string, string>("character",   toonName)
        ]);

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PoE API returned {(int)response.StatusCode}");

        var raw = await response.Content.ReadAsStringAsync();
        
        // log raw json output
        logger.LogInformation("Raw PoE API response: {Json}", raw);

        var options = new JsonSerializerOptions
        {
            // shenanigans so json gets properly mapped to the constructor's args
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        return await response.Content.ReadFromJsonAsync<PoeToon>(options)
               ?? throw new InvalidOperationException("Could not deserialize PoE API's response");
    }
}