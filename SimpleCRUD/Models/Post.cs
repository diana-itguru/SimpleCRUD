using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleCRUD.Models;

public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ProductCard Product { get; set; } = new();
}