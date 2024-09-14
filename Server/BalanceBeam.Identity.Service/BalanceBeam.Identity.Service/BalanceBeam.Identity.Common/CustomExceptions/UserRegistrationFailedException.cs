namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// Class for user registration failed exception
    /// </summary>
    public class UserRegistrationFailedException : Exception
    {
        /// <summary>
        /// Constructor for user registration failed exception
        /// </summary>
        /// <param name="errors">The errors for when user registration failed</param>
        public UserRegistrationFailedException(string errors) : base($"User registration failed with the following errors:\n{errors}")
        {
            
        }

        /// <summary>
        /// Constructor for user registration failed exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public UserRegistrationFailedException(string message, Exception innerException) 
            : base($"User registration failed with message:\n{message}", innerException)
        {
            
        }
    }
}
