using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LoggingService.Contexts;
using LoggingService.Handlers;
using LoggingService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace LoggingService.BackgroundServices
{
    public class MessageReceiverService : BackgroundService
    {
        private readonly ILogger _logger;
        private  string _queueName;
        private  QueueContext _queueContext;
        private  IConnection _connection;
        private  IModel _channel;
        private readonly UserLogMessageEventHandler _userLogMessageEventHandler;
        private readonly SystemLogMessageEventHandler _systemLogMessageEventHandler;


        public MessageReceiverService(ILogger<MessageReceiverService> logger,
            UserLogMessageEventHandler userLogMessageEventHandler,
            SystemLogMessageEventHandler systemLogMessageEventHandler)
        {
            _logger = logger;
            _userLogMessageEventHandler =  userLogMessageEventHandler;
            _systemLogMessageEventHandler = systemLogMessageEventHandler;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _queueContext = new QueueContext();
            _queueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE");
            _queueContext.Factory.DispatchConsumersAsync = true;
            _connection = _queueContext.Factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclarePassive(_queueName);
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation($"Queue [{_queueName}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                    // _logger.LogInformation($"Processing msg: '{message}'.");
                try
                {
                    var msgData = JsonConvert.DeserializeObject<List<LogMessage>>(message);

                    IEnumerable<LogMessage> userLogMessages = msgData.Where(_ => _.LogType == LogType.UserBehavior);
                    IEnumerable<LogMessage> systemLogMessages = msgData.Where(_ => _.LogType == LogType.SystemLog);

                    if (userLogMessages.Any()) await _userLogMessageEventHandler.Handler(userLogMessages);
                    if (systemLogMessages.Any()) await _systemLogMessageEventHandler.Handler(systemLogMessages);

                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, nameof(MessageReceiverService));
                }
                catch (AlreadyClosedException ex)
                {
                    _logger.LogError(ex, nameof(MessageReceiverService));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, nameof(MessageReceiverService));
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
}