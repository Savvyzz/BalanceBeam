namespace BalanceBeam.Identity.Common.CustomExceptions
{
    using System;

    /// <summary>
    /// The create JWT exception
    /// </summary>
    public class CreateJwtException : Exception
    {
        /// <summary>
        /// Constructor for the create JWT exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public CreateJwtException(string message, Exception innerException)
            : base($"Token creation for user failed with message:\n{message}", innerException)
        {
            
        }

        /// <summary>
        /// Constructor for the create JWT exception
        /// </summary>
        /// <param name="message">The exception message</param>
        public CreateJwtException(string message) : base($"Token generation failed:\n{message}")
        {
            
        }
    }
}
