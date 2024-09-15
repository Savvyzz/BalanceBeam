namespace BalanceBeam.Identity.Service.Controllers
{
    using BalanceBeam.Identity.BusinessLogic.Services;
    using BalanceBeam.Identity.Service.DTOs;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using OpenTelemetry.Trace;
    
    using System.Diagnostics;
    using System.Reflection;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            bool userRegistered = await _authenticationService.Register(userDto.Email, userDto.UserName, userDto.Password);

            if (!userRegistered)
            {
                return BadRequest("Could not register user!");
            }

            return new CreatedResult(this.Url.ToString(), userRegistered);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInUserDto userDto)
        {
            string token = await _authenticationService.Login(userDto.UserName, userDto.Password);

            return Ok(new { Token = token });
        }

        [HttpPut("modifyuser")]
        public async Task<IActionResult> UpdateUser(ModifyUserDto userDto)
        {
            var user = new IdentityUser<int>()
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Id = userDto.Id
            };

            bool userUpdated = await _authenticationService.UpdateUser(user);

            if (!userUpdated)
            {
                return BadRequest("Could not update user!");
            }

            return new CreatedResult(this.Url.ToString(), userUpdated);
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordDto userDto)
        {
            bool passwordChanged = await _authenticationService.ChangeUserPassword(userDto.UserName, userDto.CurrentPassword, userDto.NewPassword);

            if (!passwordChanged)
            {
                return BadRequest("Could not change user's password");
            }

            return Ok(passwordChanged);
        }
    }
}
