namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// Class for user signin failed exception
    /// </summary>
    public class UserSignInFailedException : Exception
    {
        /// <summary>
        /// Constructor for user sign in failed exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public UserSignInFailedException(string message, Exception innerException) : base($"User sign in failed with message:\n{message}", innerException)
        {
            
        }
    }
}
