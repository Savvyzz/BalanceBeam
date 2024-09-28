namespace BalanceBeam.Identity.BusinessLogic.Services
{
    using BalanceBeam.Identity.Common.Options;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    using System.Text;

    /// <summary>
    /// The RabbitMQ message service class
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IAsyncConnectionFactory _connectionFactory;
        private readonly IOptions<RabbitMQOptions> _options;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        /// <summary>
        /// Constructor for the RabbitMQ message service
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="options">The options</param>
        public MessageService(IAsyncConnectionFactory connectionFactory, IOptions<RabbitMQOptions> options)
        {
            _connectionFactory = connectionFactory;
            _options = options;
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        /// <inheritdoc />
        public async Task SendEmailMessage(string message)
        {
            string exchangeName = _options.Value.Exchanges["EmailExchange"];

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            
            byte[] messageBody = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: string.Empty,
                    basicProperties: null,
                    body: messageBody);
            });
        }
    }
}
