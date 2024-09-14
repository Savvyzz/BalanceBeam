namespace BalanceBeam.Identity.Service.DTOs
{
    /// <summary>
    /// The user sign in dto
    /// </summary>
    public class SignInUserDto
    {
        /// <summary>
        /// The user's username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        public string Password { get; set; }
    }
}
