using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    // Task - Represents an asynchronous operation that can return a value.
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUsers()
    {
        var users = await _userRepository.GetMembersAsync();

        return Ok(users);
    }

    [HttpGet("{username}")] // /api/users/bob
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut] // dont need to return anything to user
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) {
        // FindFirst() has ArgumentNullException so we put optional operator (?) to prevent having an exception if username is null or we dont have claim with name identifier
        var username = User.GetUsername();
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        // use mapper functionality to update the properties
        // overwrite value in user entity with updated value from memberUpdateDto
        _mapper.Map(memberUpdateDto, user);

        // return 204, everything is okay but nothing to be sent to client
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user!");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        // map into PhotoDto from photo
        if (await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);

        return BadRequest("Problem adding photo!");
    }

    // [HttpGet("{id}")] // /api/users/2
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     var user = await _context.Users.FindAsync(id);

    //     return user;
    // }
}