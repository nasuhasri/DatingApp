using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    // Task - Represents an asynchronous operation that can return a value.
    public async Task<ActionResult<PagedList<MemberDto>>> GetAllUsers([FromQuery]UserParams userParams)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        userParams.CurrentUsername = currentUser.UserName;

        if (string.IsNullOrEmpty(userParams.Gender)) {
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
        }

        var users = await _userRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [Authorize(Roles = "Member")]
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

        // set first photo of user as isMain=true
        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        // map into PhotoDto from photo
        if (await _userRepository.SaveAllAsync()) {
            // return 201 response
            // CreatedAtAction(string? actionName, object? routeValues, object? value)
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding photo!");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId) {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo!");

        // return current main photo
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        // switch main photos
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting the main photo!");
    }

    [HttpDelete("delete-photo/{photoId}")] // not returning anything to the user
    public async Task<ActionResult> DeletePhoto(int photoId) {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo!");

        // we have images that dont have public ID in database which that is the one we seed it
        // and we do not have to delete it bcs they are not in Cloudinary
        if (photo.PublicId != null) {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo!");
    }
    

    // [HttpGet("{id}")] // /api/users/2
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     var user = await _context.Users.FindAsync(id);

    //     return user;
    // }
}