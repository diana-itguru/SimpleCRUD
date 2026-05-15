using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleCRUD.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Post> CreatedPosts { get; set; } = new();
}