namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// Class for user already exists exception
    /// </summary>
    public class UserAlreadyExistsException : Exception
    {
        /// <summary>
        /// Constructor for user already exists exception
        /// </summary>
        public UserAlreadyExistsException() : base("A user already exists with the provided credentials!")
        {
            
        }
    }
}
