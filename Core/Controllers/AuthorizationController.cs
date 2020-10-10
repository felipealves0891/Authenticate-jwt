using Core.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<User> _manager;

        private readonly SignInManager<User> _signIn;

        private readonly IConfiguration _configuration;

        public AuthorizationController(UserManager<User> manager, SignInManager<User> signIn, IConfiguration configuration)
        {
            _manager = manager;
            _signIn = signIn;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserToken>> Register([FromBody] UserRegistration registration)
        {
            var exists = await _manager.FindByEmailAsync(registration.Email);
            if (exists != null)
                return BadRequest(new { Message = "O e-mail fornecido não esta disponivel." });

            var user = new User
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Email = registration.Email,
                UserName = ExtractEmailName(registration.Email)
            };

            var result = await _manager.CreateAsync(user, registration.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);

                return BadRequest(errors);
            }

            return BuildToken(user.Email);

        }

        [HttpPost]
        [Route("in")]
        public async Task<ActionResult<UserToken>> LogIn([FromBody] UserLogin login)
        {
            var userName = ExtractEmailName(login.Email);

            var result = await _signIn.PasswordSignInAsync(
                userName,
                login.Password,
                true,
                true);

            if (!result.Succeeded)
                return BadRequest(new { Message = "E-mail e/ou senha invalido." });

            return BuildToken(login.Email);

        }


        private UserToken BuildToken(string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(3);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        private string ExtractEmailName(string email)
        {
            if (email.IndexOf("@") <= 0)
                return email;

            var positionAtSign = email.IndexOf("@") - 1;
            return email.Substring(0, positionAtSign);
        }


    }
}
