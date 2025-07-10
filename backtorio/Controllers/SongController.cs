using backtorio.Model;
using backtorio.service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
namespace backtorio.Controllers;


[ApiController]
public class SongController : Controller
{
    private readonly IMongoCollection<Canzoni> _canzoni;
    private SpotifyController _spotify;
    public SongController(MongoDbService mds)
    {
        _canzoni = MongoDbService.Db.GetCollection<Canzoni>("Canzoni");
        
    }

    #region Get Method
    
    

    

    
    
    /*[HttpGet("FindByArtist/{artist}")]
    public async Task<OkObjectResult> FindByArtist(string artist)
    {
        try
        {
            if (artist != null)
            {
                var canzoni = await _canzoni.Find(c=> c.Artist == artist).ToListAsync();
                return Ok(canzoni);
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    
    [HttpGet("FindByTitle/{title}")]
    public async Task<OkObjectResult> FindByTitle(string title)
    {
        try
        {
            if (title != null)
            {
                var canzoni = await _canzoni.Find(c => c.Title == title).ToListAsync();
                return Ok(canzoni);
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }*/

    [HttpGet("FindBySongId/{songID}")]
    public async Task<OkObjectResult> FindBySongId(string songID)
    {
        try
        {
            if (songID != null)
            {
                var canzone = await _canzoni.Find(c=> c.SongId == songID).ToListAsync();
                
                if (canzone.Count() > 0)
                {
                    return Ok(true);
                }else
                {
                    return Ok(false);
                }
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion

    #region Post Method
    
    //create new song
    [HttpPost("AddNewSong/{songID}")]
    public async Task<IActionResult> AddNewSong(string songId, [FromBody] List<string> vote)
    {
        try
        {
            Canzoni song = new Canzoni
            {
                SongId = songId,
                Vote = vote,
                Update = DateTime.Now
            };
            await _canzoni.InsertOneAsync(song);
            return CreatedAtAction(nameof(AddNewSong),new {Id= song.id}, song);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    
    #endregion
    
    #region Put Method
    //todo aggiustare
    
    //update a song
    [HttpPut("UpdateSong/{songID}")]
    public async Task<IActionResult> UpdateSong(string songID,   [FromBody] List<string> vote)
    {
        try
        {
            var filter = Builders<Canzoni>.Filter.Eq("songID", songID);
            var update = Builders<Canzoni>.Update
                .Set(c => c.Vote, vote)
                .Set(c => c.Update, DateTime.Now);
            var options = new FindOneAndUpdateOptions<Canzoni>
            {
                ReturnDocument = ReturnDocument.After
            };
            var updatedSong = await _canzoni.FindOneAndUpdateAsync(filter, update, options);
            if (updatedSong == null)
                return NotFound("Canzone non trovata.");

            return Ok(updatedSong); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
    
    #region Delete Method


    #endregion
}

//todo
/*
 * get all
 * get aggiornati
 * aggiungi
 * modifica
 * elimina
 */