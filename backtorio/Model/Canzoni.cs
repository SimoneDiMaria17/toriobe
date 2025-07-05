using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backtorio.Model;

public class Canzoni
{
    [BsonId]
    [BsonElement("_id"),BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    
    [BsonElement("songID"),BsonRepresentation(BsonType.String)]
    public string SongId { get; set; }
    
    [BsonElement("vote")]
    public List<string> Vote { get; set; }
    
    [BsonElement("updated"),BsonRepresentation(BsonType.DateTime)]
    public DateTime Update { get; set; }
    
    
    
}