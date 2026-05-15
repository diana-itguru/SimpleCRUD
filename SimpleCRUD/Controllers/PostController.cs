using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SimpleCRUD.Models;

namespace SimpleCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IMongoCollection<Post> _postsCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public PostsController(IMongoCollection<Post> postsCollection, IMongoCollection<User> usersCollection)
    {
        _postsCollection = postsCollection;
        _usersCollection = usersCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        var posts = await _postsCollection.Find(_ => true).ToListAsync();
        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
    {
        await _postsCollection.InsertOneAsync(newPost);
        
        var filter = Builders<User>.Filter.Regex(u => u.Username, new MongoDB.Bson.BsonRegularExpression($"^{newPost.AuthorName}$", "i"));
        var update = Builders<User>.Update.Push(u => u.CreatedPosts, newPost);

        await _usersCollection.UpdateOneAsync(filter, update);

        return CreatedAtAction(nameof(GetPosts), new { id = newPost.Id }, newPost);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(string id, [FromBody] Post updatedPost)
    {
        var postResult = await _postsCollection.ReplaceOneAsync(p => p.Id == id, updatedPost);
    
        if (postResult.MatchedCount == 0) return NotFound("Пост не найден");
        
        var userFilter = Builders<User>.Filter.ElemMatch(u => u.CreatedPosts, p => p.Id == id);
        var userUpdate = Builders<User>.Update.Set("CreatedPosts.$", updatedPost);
    
        await _usersCollection.UpdateOneAsync(userFilter, userUpdate);

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var postResult = await _postsCollection.DeleteOneAsync(p => p.Id == id);
    
        if (postResult.DeletedCount == 0) return NotFound("Пост не найден");
        
        var userFilter = Builders<User>.Filter.ElemMatch(u => u.CreatedPosts, p => p.Id == id);
        var userUpdate = Builders<User>.Update.PullFilter(u => u.CreatedPosts, p => p.Id == id);
    
        await _usersCollection.UpdateOneAsync(userFilter, userUpdate);

        return NoContent();
    }
}