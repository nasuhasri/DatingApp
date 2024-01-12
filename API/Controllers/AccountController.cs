using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken!");

            // map AppUser from registerDto
            var user = _mapper.Map<AppUser>(registerDto);

            // this instances requires some spaces in memory and once we finished with the class,
            // we want to dispose it automatically - therefore using keyword
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)); // return byte array
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // we specify to return a user - will return all properties incl passwords
            // but using DTO, only return properties we want
            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) {
            // find() - if we know primary key
            // firstOrDefault() - Returns the first element of a sequence, or a default value if the sequence contains no elements.
            // first - get exception if user does not exists in db
            // singleOrDefault() - Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence
            var user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username!");

            // to get the exact same hash algorithm, pass the key to the hmac method
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password!");
            }

            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                // Entity framework is not going to load related entities by default so this will causing errors if we dont put (?)
                // we need to eagerly load photos entities as well
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}