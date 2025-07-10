using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using backtorio.Model;
using backtorio.service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backtorio.Controllers;

[ApiController]
public class SpotifyController : Controller
{
    private static IConfiguration _configuration;
    private readonly IMongoCollection<Canzoni> _canzoni;
    
    public SpotifyController(IConfiguration configuration,MongoDbService mds)
    {
        _canzoni = MongoDbService.Db.GetCollection<Canzoni>("Canzoni");
        _configuration = configuration;
    }

    private string clientId = "0bbdf68a3a2c4d168c0c81e213b3f111"; //_configuration["Setting:ClientId"];
    private string clientSecret = "b0afa1c4c2ae4b1db9180f5a29c8c16f";//_configuration["Setting:ClientSecret"];
    private string spotifUrl = "https://accounts.spotify.com/api/token";//_configuration["Setting:SpotifyUrl"];

    private string infoUrl = "https://v1.nocodeapi.com/cluadio/spotify/FDqxYHssmymUocJx/tracks?ids";
    // GET
   /* [HttpGet("GetToken")]
   async public Task<string> GetToken()
    {
        using HttpClient client = new HttpClient();
        
        string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", auth);

        var content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
            "application/x-www-form-urlencoded");
        var response = await client.PostAsync(spotifUrl, content) ;
        if (!response.IsSuccessStatusCode)
        {
            string err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Errore nella chiamata: {response.StatusCode}, {err}");
        }

        // Leggi e deserializza la risposta JSON
        var json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Prendi il token di accesso
        string accessToken = root.GetProperty("access_token").GetString();

        return accessToken;
    }*/

    [HttpGet("GetInfoById/{songId}")]
    /* async public Task<Info> GetInfoById(string songId)
     {
         using HttpClient client = new HttpClient();
         HttpResponseMessage response = await client.GetAsync($"{infoUrl}={songId}");
         response.EnsureSuccessStatusCode();
         var json = await response.Content.ReadAsStringAsync();

         using JsonDocument doc = JsonDocument.Parse(json);
         var root = doc.RootElement;
         var track = root.GetProperty("tracks")[0];

         Info info = new Info
         {
             Artist = track.GetProperty("artists")[0].GetProperty("name").GetString(),
             Album = track.GetProperty("album").GetProperty("name").GetString(),
             Title = track.GetProperty("name").GetString(),
             img = track.GetProperty("album").GetProperty("images")[0].GetProperty("url").GetString()
         };
         return info;
     }*/
    public async Task<ObjectResult> GetInfoById(string songId)
    {
        string? token = await GetSpotifyAccessToken();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/tracks/{songId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                int retryAfter = int.Parse(values.First());
                await Task.Delay(retryAfter * 1000);
                
                var retryRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/tracks/{songId}");
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                response = await client.SendAsync(request);
            }
        }
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Errore Spotify: {error}");
        }

        var content = await response.Content.ReadAsStringAsync();
        
        using JsonDocument doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        //var tracks = root.GetProperty("tracks");
        
        Info info = new Info
        {
            Artist = root.GetProperty("artists")[0].GetProperty("name").GetString(),
            Album = root.GetProperty("album").GetProperty("name").GetString(),
            Title = root.GetProperty("name").GetString(),
            img = root.GetProperty("album").GetProperty("images")[0].GetProperty("url").GetString()
        };
        return Ok(info);
    }

    [HttpGet("GetSpotifyAccessToken")]
    public async Task<string?> GetSpotifyAccessToken()
    {
        var client = new HttpClient();
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Errore nel recupero del token: {content}");

        var json = JsonDocument.Parse(content);
        var token = json.RootElement.GetProperty("access_token").GetString();
        return token;
    }

    [HttpGet("GetCompleteSongs")]
    public async Task<List<Info>> GetCompleteSong()
    {
        List<Info> Canzoni = new List<Info>();
        List<Canzoni> canzoni = await _canzoni.Find(c => true).ToListAsync();
        foreach (var c in canzoni)
        {
            var luca = await GetInfoById(c.SongId);
            Info? informazioni = (Info)luca.Value!;

            informazioni.id = c.id;
            informazioni.SongId = c.SongId;
            informazioni.Vote = c.Vote;
            informazioni.Update = c.Update;
            Canzoni.Add(informazioni);
        }
        return Canzoni;
    }
    
    [HttpGet("GetMostVoted")]
    public async Task<OkObjectResult> GetMostVoted()
    {
        try
        {
            List<Info> Canzoni = new List<Info>();
            List<Canzoni> canzoni = await _canzoni.Find(c => true).SortByDescending(c=>c.Vote).ToListAsync();
            foreach (var c in canzoni)
            {
                var luca = await GetInfoById(c.SongId);
                Info? informazioni = (Info)luca.Value!;

                informazioni.id = c.id;
                informazioni.SongId = c.SongId;
                informazioni.Vote = c.Vote;
                informazioni.Update = c.Update;
                Canzoni.Add(informazioni);
            }
            return Ok(canzoni);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpGet("GetUpdated")]
    public async Task<OkObjectResult> GetUpdated()
    {
        try
        {
            List<Info> Canzoni = new List<Info>();
            List<Canzoni> canzoni = await _canzoni.Find(c => true).SortByDescending(c=> c.Update).ToListAsync();
           
            foreach (var c in canzoni)
            {
                var luca = await GetInfoById(c.SongId);
                Info? informazioni = (Info)luca.Value!;

                informazioni.id = c.id;
                informazioni.SongId = c.SongId;
                informazioni.Vote = c.Vote;
                informazioni.Update = c.Update;
                Canzoni.Add(informazioni);
            }
            
            return Ok(canzoni);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
}

