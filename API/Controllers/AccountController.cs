using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context; 
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto) {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken!");

            // this instances requires some spaces in memory and once we finished with the class,
            // we want to dispose it automatically - therefore using keyword
            using var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), // return byte array
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // we specify to return a user - will return all properties incl passwords
            // but using DTO, only return properties we want
            return user; 
        }

        private async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}