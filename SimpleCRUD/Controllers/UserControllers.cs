using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SimpleCRUD.Models;

namespace SimpleCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<Post> _postsCollection;

    public UsersController(IMongoCollection<User> usersCollection, IMongoCollection<Post> postsCollection)
    {
        _usersCollection = usersCollection;
        _postsCollection = postsCollection;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _usersCollection.Find(_ => true).ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
    {
        var authorPosts = await _postsCollection
            .Find(p => p.AuthorName.ToLower() == newUser.Username.ToLower())
            .ToListAsync();

        newUser.CreatedPosts = authorPosts;

        await _usersCollection.InsertOneAsync(newUser);
        return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
    }
}