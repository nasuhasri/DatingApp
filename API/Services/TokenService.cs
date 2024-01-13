using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Entities;
using API.interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // receives from IdentityModel.Tokens
        // 2 types of keys: symmetric and asymmetric
        // symmetric: the same key is used to encrypt data and decrypt data
        // asymmetric: server needs to encrypt smthg and client needs to decrypt smthg (public and private key)
        // private key - stays on server, public key - used to decrypt data
        private readonly SymmetricSecurityKey _key; // stays on server and never go to client

        public TokenService(IConfiguration config)
        {
            // return byte array
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // claim - a bit of information that a user claims to be his/her
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // the token we're going to return
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}