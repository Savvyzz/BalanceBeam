namespace BalanceBeam.Identity.Service.DTOs
{
    /// <summary>
    /// The register user dto
    /// </summary>
    public class RegisterUserDto
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
        /// The user's password
        /// </summary>
        public string Password { get; set; }
    }
}
