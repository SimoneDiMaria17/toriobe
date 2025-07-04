using backtorio.Model;
using backtorio.service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
namespace backtorio.Controllers;


[ApiController]
public class SongController : Controller
{
    private readonly IMongoCollection<Canzoni> _canzoni;
    public SongController(MongoDbService mds)
    {
        _canzoni = MongoDbService.Db.GetCollection<Canzoni>("Canzoni");
    }

    #region Get Method
    
    [HttpGet("GetAll")]
    public async Task<OkObjectResult> GetAll()
    {
        try
        {
            var Canzoni = await _canzoni.Find(c => true).ToListAsync();
            return Ok(Canzoni);
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
            var canzoni = await _canzoni.Find(c => true).SortByDescending(c=> c.Update).ToListAsync();
            return Ok(canzoni);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    [HttpGet("GetMostVoted")]
    public async Task<OkObjectResult> GetMostVoted()
    {
        try
        {
             var canzoni = await _canzoni.Find(c => true).SortByDescending(c=>c.Vote).ToListAsync();
             return Ok(canzoni);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
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
    [HttpPost("AddNewSong/{songID}/{vote}")]
    public async Task<IActionResult> AddNewSong(string songId,  int vote)
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
    
    [HttpPost("AddsNewSongsFromJson")]
    public async Task<IActionResult> AddsNewSongsFromJson([FromBody] Canzoni canzoni)
    {
        try
        {
            await _canzoni.InsertOneAsync(canzoni);
            return CreatedAtAction(nameof(AddNewSong),new {Id= canzoni.id}, canzoni);
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
    [HttpPut("UpdateSong/{songID}/{vote}")]
    public async Task<IActionResult> UpdateSong(string songID, int vote)
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