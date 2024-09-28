namespace BalanceBeam.Identity.BusinessLogic.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the RabbitMQ message service
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Sends message to email service through RabbitMQ
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        Task SendEmailMessage(string message);
    }
}
