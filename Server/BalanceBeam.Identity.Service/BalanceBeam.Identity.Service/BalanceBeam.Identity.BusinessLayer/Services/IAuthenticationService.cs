namespace BalanceBeam.Identity.BusinessLogic.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Interface for the authentication service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="userName">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns></returns>
        public Task<bool> Register(string email, string userName, string password);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">The <see cref="IdentityUser"/></param>
        /// <returns></returns>
        public Task<IdentityUser<int>> UpdateUser(IdentityUser<int> user);

        /// <summary>
        /// Sings in a user
        /// </summary>
        /// <param name="userName">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns>The token if sign in success, otherwise throws <see cref="UnauthorizedAccessException"/></returns>
        public Task<string> Login(string userName, string password);

        /// <summary>
        /// Changes the user's password
        /// </summary>
        /// <param name="userName">The user's username</param>
        /// <param name="currentPassword">The user's current password</param>
        /// <param name="newPassword">The user's new password</param>
        /// <returns></returns>
        public Task<bool> ChangeUserPassword(string userName, string currentPassword, string newPassword);
    }
}
