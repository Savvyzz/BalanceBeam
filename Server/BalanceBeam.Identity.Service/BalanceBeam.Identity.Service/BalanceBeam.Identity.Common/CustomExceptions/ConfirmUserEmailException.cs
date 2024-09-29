namespace BalanceBeam.Identity.Common.CustomExceptions
{
    /// <summary>
    /// Custom exception for confirm user email exception
    /// </summary>
    public class ConfirmUserEmailException : Exception
    {
        /// <summary>
        /// Constructor for the confirm user email exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ConfirmUserEmailException(string message, Exception innerException) 
            : base($"Confirm user email failed with message:{message}", innerException)
        {
            
        }
    }
}
