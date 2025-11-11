using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicPlaylistAPI.Models.Entity;

public class Playlist
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }
}
