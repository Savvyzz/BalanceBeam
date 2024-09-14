namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// Class for user not found exception
    /// </summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>
        /// Constructor for user not found exception
        /// </summary>
        public UserNotFoundException() : base("No user was found with the provided information!")
        {
            
        }
    }
}
