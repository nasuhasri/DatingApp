using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AppUser>> Register(string username, string password) {
            // this instances requires some spaces in memory and once we finished with the class,
            // we want to dispose it automatically - therefore using keyword
            using var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), // return byte array
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user; // we specify to return a user
        }
    }
}