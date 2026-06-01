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
        
        // Optional
        var sessId = config["PathOfExile:SessionId"];

        // i hate var i regret everything
        // todo string validation for the above in separate method
        
        var handler = new HttpClientHandler() { UseCookies = false };
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("User-Agent", "WebAPI/1.0.0 (contact: test@test.com) PoeLocalTest");
        
        //Console.WriteLine($"Successfully loaded Session ID: {usrPoeSessId[..5]}..."); // test if SSID output works

        // get poe character's items list with mods
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://www.pathofexile.com/character-window/get-items");
        
        // Add session cookie dynamically if found in local secrets
        if (!string.IsNullOrEmpty(sessId))
        {
            request.Headers.Add("Cookie", $"POESESSID={sessId}");
            logger.LogInformation("POESESSID found. Sending authenticated request for account: {Account}", accountName);
        }
        else
        {
            logger.LogInformation("No POESESSID found. Attempting unauthenticated request for account: {Account}", accountName);
        }

        request.Content = new FormUrlEncodedContent
        ([
            new KeyValuePair<string, string>("accountName", accountName),
            new KeyValuePair<string, string>("character",   toonName)
        ]);

        var response = await client.SendAsync(request);

        // Throw exception in case of private profile or request denial by the PoE API
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException($"Cannot access profile. The account '{accountName}' is private or requires a valid POESESSID.");
        }

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