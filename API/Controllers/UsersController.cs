using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")] // /api/users
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    // Task - Represents an asynchronous operation that can return a value.
    public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
    {
        return Ok(await _userRepository.GetUsersAsync());
    }

    [HttpGet("{username}")] // /api/users/bob
    public async Task<ActionResult<AppUser>> GetUser(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);

        return user;
    }

    // [HttpGet("{id}")] // /api/users/2
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     var user = await _context.Users.FindAsync(id);

    //     return user;
    // }
}