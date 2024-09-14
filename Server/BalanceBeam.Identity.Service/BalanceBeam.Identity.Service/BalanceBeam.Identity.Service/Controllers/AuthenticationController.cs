namespace BalanceBeam.Identity.Service.Controllers
{
    using BalanceBeam.Identity.BusinessLogic.Services;
    using BalanceBeam.Identity.Service.DTOs;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

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
            try
            {
                bool userRegistered = await _authenticationService.Register(userDto.Email, userDto.UserName, userDto.Password);

                if (!userRegistered)
                {
                    return BadRequest("Could not register user");
                }

                return new CreatedResult(this.Url.ToString(), userRegistered);
            } catch(Exception ex)
            {
                // TODO: Add tracing
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInUserDto userDto)
        {
            try
            {
                string token = await _authenticationService.Login(userDto.UserName, userDto.Password);
                
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                // TODO: Add tracing
                return Unauthorized("Invalid login attempt");
            }
            catch (Exception ex)
            {
                // TODO: Add tracing
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("modifyuser")]
        public async Task<IActionResult> UpdateUser(ModifyUserDto userDto)
        {
            try
            {
                var user = new IdentityUser<int>()
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email
                };

                IdentityUser<int> userRegistered = await _authenticationService.UpdateUser(user);

                return new CreatedResult(this.Url.ToString(), user);
            }
            catch (Exception ex)
            {
                // TODO: Add tracing
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordDto userDto)
        {
            try
            {
                bool passwordChanged = await _authenticationService.ChangeUserPassword(userDto.UserName, userDto.CurrentPassword, userDto.NewPassword);

                if (!passwordChanged)
                {
                    return BadRequest("Could not change user's password");
                }

                return Ok(passwordChanged);
            }
            catch (Exception ex)
            {
                // TODO: Add tracing
                return BadRequest(ex.Message);
            }
        }
    }
}
