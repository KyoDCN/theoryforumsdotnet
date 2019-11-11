using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheoryForums.Shared.DataTransferObjects;
using TheoryForums.Shared.Helpers;
using TheoryForums.Shared.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TheoryForums.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Config;
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _SignInManager;

        public AuthController(IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _Config = config;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterCreateDTO userCreateDTO)
        {
            User newUser = new User()
            {
                UserName = userCreateDTO.UserName,
                DisplayName = userCreateDTO.DisplayName,
                Email = userCreateDTO.Email,
                JoinDate = DateTime.Now
            };

            IdentityResult result = await _UserManager.CreateAsync(newUser, userCreateDTO.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            result = await _UserManager.AddToRoleAsync(newUser, "Member");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { token = await GenerateLoginTokenAsync(newUser), newUser.AvatarUrl });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            User user = await _UserManager.FindByNameAsync(userLoginDTO.UserName);

            if (user == null)
                return BadRequest("Incorrect Username or Password");

            // Log out all other sessions
            await _UserManager.UpdateSecurityStampAsync(user);

            SignInResult result = await _SignInManager.CheckPasswordSignInAsync(user, userLoginDTO.Password, false);

            if (!result.Succeeded)
                return BadRequest("Incorrect Username or Password");

            return Ok(new { token = await GenerateLoginTokenAsync(user), user.AvatarUrl });
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] string email)
        {
            var user = await _UserManager.FindByEmailAsync(email);
            var resetToken = await _UserManager.GeneratePasswordResetTokenAsync(user);

            return Ok();
        }

        private async Task<string> GenerateLoginTokenAsync(User user)
        {
            List<Claim> claims = (await _SignInManager.CreateUserPrincipalAsync(user)).Claims.ToList();

            for (int i = 0; i < claims.Count; i++)
            {
                if( claims[i].Type == ClaimTypes.Name)
                {
                    claims[i] = new Claim(ClaimTypes.Name, user.DisplayName);
                    break;
                }
            }

            var secret = _Config.GetSection("AppSettings:Secret").Value;

            return new JwtSecurityTokenHandler().GenerateToken(claims, secret, _Config);
        }
    }
}