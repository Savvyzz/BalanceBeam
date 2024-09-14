namespace BalanceBeam.Identity.Service.DTOs
{
    /// <summary>
    /// The change user password dto
    /// </summary>
    public class ChangeUserPasswordDto
    {
        /// <summary>
        /// The user's username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's current password
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The user's new password
        /// </summary>
        public string NewPassword { get; set; }
    }
}
