namespace BalanceBeam.Identity.Common.Options
{
    /// <summary>
    /// The RabbitMQ options class
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// The RabbitMQ hostname
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The RabbitMQ username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The RabbitMQ password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The RabbitMQ exchanges
        /// </summary>
        public Dictionary<string, string> Exchanges { get; set; }
    }
}
