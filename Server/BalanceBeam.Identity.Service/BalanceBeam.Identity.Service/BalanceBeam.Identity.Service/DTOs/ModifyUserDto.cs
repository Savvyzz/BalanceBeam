namespace BalanceBeam.Identity.Service.DTOs
{
    // The modify user dto
    public class ModifyUserDto
    {
        /// <summary>
        /// The user's id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user's username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user's email address
        /// </summary>
        public string Email { get; set; }
    }
}
