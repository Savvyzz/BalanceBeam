namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// Class for user update failed exception
    /// </summary>
    public class UserUpdateFailedException : Exception
    {
        /// <summary>
        /// Constructor for user update failed exception
        /// </summary>
        /// <param name="errors">The errors for when user update failed</param>
        public UserUpdateFailedException(string errors) : base($"User update failed with the following errors:\n{errors}")
        {
            
        }

        /// <summary>
        /// Constructor for user update failed exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public UserUpdateFailedException(string message, Exception innerException) 
            : base($"User update failed with message:{message}", innerException)
        {

        }
    }
}
