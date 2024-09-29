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

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInUserDto userDto)
        {
            string token = await _authenticationService.Login(userDto.UserName, userDto.Password);

            return Ok(new { Token = token });
        }

        [HttpPut("modify-user")]
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

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordDto userDto)
        {
            bool passwordChanged = await _authenticationService.ChangeUserPassword(userDto.UserName, userDto.CurrentPassword, userDto.NewPassword);

            if (!passwordChanged)
            {
                return BadRequest("Could not change user's password");
            }

            return Ok(passwordChanged);
        }

        [HttpPost("confirm-email/{userName}/{token}")]
        public async Task<IActionResult> ConfirmUserEmail(string userName, string token)
        {
            bool emailConfirmed = await _authenticationService.ConfirmEmail(userName, token);

            if (!emailConfirmed)
            {
                return BadRequest("Could not confirm user email");
            }

            return Ok(emailConfirmed);
        }
    }
}
