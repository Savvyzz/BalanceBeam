namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// The chage user password exception
    /// </summary>
    public class ChangeUserPasswordException : Exception
    {

        /// <summary>
        /// Constructor for the change user password exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ChangeUserPasswordException(string message, Exception innerException) 
            : base($"Change password for user failed with message:\n{message}", innerException)
        {
            
        }
    }
}
