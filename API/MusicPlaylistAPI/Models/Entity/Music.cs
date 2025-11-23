using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicPlaylistAPI.Models.Entity;

public class Music
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("artist")]
    public string Artist { get; set; }

    [BsonElement("link")]
    public string Link { get; set; }

    [BsonElement("playlistId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PlaylistId { get; set; }
}
