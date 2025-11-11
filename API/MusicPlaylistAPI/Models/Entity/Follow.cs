using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicPlaylistAPI.Models.Entity;

public class Follow
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("follower")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    [BsonElement("followedAt")]
    public DateTime FollowedAt { get; set; }

    [BsonElement("playlist")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PlaylistId { get; set; }
}
