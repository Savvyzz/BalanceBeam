namespace BalanceBeam.Identity.BusinessLogic.Services
{
    using BalanceBeam.Identity.Common.Contracts;
    using BalanceBeam.Identity.Common.CustomExceptions;
    using BalanceBeam.Identity.Common.Helpers.OTL;
    using MassTransit;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using OpenTelemetry.Trace;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// The authentication service
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthenticationService(UserManager<IdentityUser<int>> userManager, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }

        /// <inheritdoc />
        public async Task<string> Login(string userName, string password)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Sign in"))
            {
                try
                {
                    var tags = new ActivityTagsCollection
                    {
                        { OTLHelper.AttributeUserName, userName }
                    };

                    IdentityUser<int>? existingUser = await _userManager.FindByNameAsync(userName);

                    if (existingUser == null || !await _userManager.CheckPasswordAsync(existingUser, password))
                    {
                        activity?.AddEvent(
                            new ActivityEvent(
                                "User sign in failed. User not found or credential missmatch",
                                DateTimeOffset.UtcNow,
                                tags)
                            );

                        throw new UnauthorizedAccessException("Invalid login credentials");
                    }

                    var token = await GenerateJwtToken(existingUser);

                    activity?.AddEvent(new ActivityEvent("User signed in", DateTimeOffset.UtcNow, tags));

                    return token;
                }
                catch (UnauthorizedAccessException ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new UserSignInFailedException(ex.Message, ex);
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> Register(string email, string userName, string password)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Register"))
            {
                try
                {
                    var tags = new ActivityTagsCollection
                    {
                        { OTLHelper.AttributeUserName, userName },
                        { OTLHelper.AttributeUserEmail, email }
                    };

                    IdentityUser<int>? existingUserByEmail = await _userManager.FindByEmailAsync(email);
                    IdentityUser<int>? existingUserByUsername = await _userManager.FindByNameAsync(userName);

                    if (existingUserByEmail != null || existingUserByUsername != null)
                    {
                        activity?.AddEvent(
                            new ActivityEvent(
                                "User registration failed. User already exists with the provided username, and or email.", 
                                DateTimeOffset.UtcNow, 
                                tags)
                            );

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
                        tags.Add(OTLHelper.AttributeRegisterFailed, result.Errors);

                        activity?.AddEvent(new ActivityEvent("User not registered", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string encodedToken = HttpUtility.UrlEncode(token);

                    var message = new UserRegisteredEvent(userName, email, encodedToken);

                    await _publishEndpoint.Publish(message);

                    activity?.AddEvent(new ActivityEvent("User registered", DateTimeOffset.UtcNow, tags));

                    return true;
                }
                catch (Exception ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new UserRegistrationFailedException(ex.Message, ex);
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUser(IdentityUser<int> user)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Update user info"))
            {
                try
                {
                    var tags = new ActivityTagsCollection()
                    {
                        { OTLHelper.AttributeUserName, user.UserName },
                        { OTLHelper.AttributeUserEmail, user.Email },
                        { OTLHelper.AttributeUserId, user.Id },
                    };

                    IdentityUser<int>? existingUser = await _userManager.FindByNameAsync(user.UserName);

                    if (existingUser == null)
                    {
                        activity?.AddEvent(new ActivityEvent("User does not exist", DateTimeOffset.UtcNow, tags));

                        throw new UserNotFoundException();
                    }

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        tags.Add(OTLHelper.AttributeUpdateFailed, result.Errors);

                        activity?.AddEvent(new ActivityEvent("User update failed", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    activity?.AddEvent(new ActivityEvent("User updated successfully", DateTimeOffset.UtcNow, tags));

                    return true;
                }
                catch (Exception ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new UserUpdateFailedException(ex.Message, ex);
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> ChangeUserPassword(string userName, string currentPassword, string newPassword)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Change user password"))
            {
                try
                {
                    var tags = new ActivityTagsCollection
                    {
                        { OTLHelper.AttributeUserName, userName }
                    };

                    IdentityUser<int>? user = await _userManager.FindByNameAsync(userName);

                    if (user == null)
                    {
                        activity?.AddEvent(new ActivityEvent("User does not exist", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                    if (!result.Succeeded)
                    {
                        tags.Add(OTLHelper.AttributeUpdateFailed, result.Errors);

                        activity?.AddEvent(new ActivityEvent("Change user password failed", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    activity?.AddEvent(new ActivityEvent("Change user password successfull", DateTimeOffset.UtcNow, tags));

                    return true;
                }
                catch (Exception ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new ChangeUserPasswordException(ex.Message, ex);
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> ConfirmEmail(string userName, string token)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Confirm Email"))
            {
                try
                {
                    var tags = new ActivityTagsCollection
                    {
                        { OTLHelper.AttributeUserName, userName }
                    };

                    IdentityUser<int>? user = await _userManager.FindByNameAsync(userName);

                    if (user == null)
                    {
                        activity?.AddEvent(new ActivityEvent("User does not exist", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    string decodedToken = HttpUtility.UrlDecode(token);

                    IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                    if (!result.Succeeded)
                    {
                        tags.Add(OTLHelper.AttributeUpdateFailed, result.Errors);

                        activity?.AddEvent(new ActivityEvent("Confirm email failed", DateTimeOffset.UtcNow, tags));

                        return false;
                    }

                    activity?.AddEvent(new ActivityEvent("Confirm email successfull", DateTimeOffset.UtcNow, tags));

                    return true;
                }
                catch (Exception ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new ConfirmUserEmailException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Generates JWT token for when user signs in
        /// </summary>
        /// <param name="user">The <see cref="IdentityUser"/></param>
        /// <returns>A JWT token for the signed in user</returns>
        public async Task<string> GenerateJwtToken(IdentityUser<int> user)
        {
            using (var activity = new ActivitySource(OTLHelper.ActivitySource).StartActivity("Generate JWT"))
            {
                try
                {
                    var tags = new ActivityTagsCollection
                    {
                        { OTLHelper.AttributeUserEmail, user.Email },
                        { OTLHelper.AttributeUserName, user.UserName },
                    };

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

                    if(token == null)
                    {
                        activity?.AddEvent(new ActivityEvent("JWT token not generated for user", DateTimeOffset.UtcNow, tags));

                        throw new CreateJwtException("Token not created");
                    }

                    activity?.AddEvent(new ActivityEvent("JWT token generated for user", DateTimeOffset.UtcNow, tags));

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                catch (Exception ex)
                {
                    var tags = new TagList
                    {
                        { OTLHelper.AttributeExceptionMessage, ex.Message },
                        { OTLHelper.AttributeExceptionType, ex.GetType().FullName },
                        { OTLHelper.AttributeExceptionStacktrace, ex.StackTrace }
                    };

                    activity?.RecordException(ex, tags);

                    throw new CreateJwtException(ex.Message, ex);
                }
            }
        }
    }
}
