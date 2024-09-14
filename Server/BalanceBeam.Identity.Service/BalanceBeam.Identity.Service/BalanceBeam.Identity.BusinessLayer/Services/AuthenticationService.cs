namespace BalanceBeam.Identity.BusinessLogic.Services
{
    using BalanceBeam.Identity.Common.CustomExceptions;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The authentication service
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<IdentityUser<int>> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<string> Login(string userName, string password)
        {
            try
            {
                IdentityUser<int>? existingUser = await _userManager.FindByNameAsync(userName);

                if (existingUser == null || !await _userManager.CheckPasswordAsync(existingUser, password))
                {
                    throw new UnauthorizedAccessException("Invalid login credentials");
                }

                var token = await GenerateJwtToken(existingUser);

                return token;
            } 
            catch(Exception ex)
            {
                // TODO: Add tracing
                throw new UserSignInFailedException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> Register(string email, string userName, string password)
        {
            try
            {
                IdentityUser<int>? existingUserByEmail = await _userManager.FindByEmailAsync(email);
                IdentityUser<int>? existingUserByUsername = await _userManager.FindByNameAsync(userName);

                if (existingUserByEmail != null || existingUserByUsername != null)
                {
                    throw new UserAlreadyExistsException();
                }

                var user = new IdentityUser<int>()
                {
                    UserName = userName,
                    Email = email
                };

                IdentityResult result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return false;
                }

                return true;
            } 
            catch(Exception ex)
            {
                // TODO: Add tracing
                throw new UserRegistrationFailedException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<IdentityUser<int>> UpdateUser(IdentityUser<int> user)
        {
            try
            {
                IdentityUser<int>? existingUser = await _userManager.FindByNameAsync(user.UserName);

                if (existingUser == null)
                {
                    throw new UserNotFoundException();
                }

                IdentityResult updateInfo = await _userManager.UpdateAsync(user);

                if (!updateInfo.Succeeded)
                {
                    throw new UserUpdateFailedException(updateInfo.Errors.ToString());
                }

                return user;
            } 
            catch(Exception ex)
            {
                // TODO: Add tracing
                throw new UserUpdateFailedException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ChangeUserPassword(string userName, string currentPassword, string newPassword)
        {
            IdentityUser<int>? user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return false;
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates JWT token for when user signs in
        /// </summary>
        /// <param name="user">The <see cref="IdentityUser"/></param>
        /// <returns>A JWT token for the signed in user</returns>
        private async Task<string> GenerateJwtToken(IdentityUser<int> user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
