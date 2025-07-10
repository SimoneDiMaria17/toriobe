using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backtorio.Model;

public class Info:Canzoni
{
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? img { get; set; }
}