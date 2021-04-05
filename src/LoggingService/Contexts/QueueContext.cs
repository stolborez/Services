using System;
using RabbitMQ.Client;

namespace LoggingService.Contexts
{
    public class QueueContext
    {
        public ConnectionFactory Factory { get; set; }

        public QueueContext()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME")))
                throw new Exception($"Cannot parse field RABBITMQ_HOSTNAME");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_PORT")))
                throw new Exception($"Cannot parse field RABBITMQ_PORT");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")))
                throw new Exception($"Cannot parse field RABBITMQ_USERNAME");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")))
                throw new Exception($"Cannot parse field RABBITMQ_PASSWORD");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_VIRTUALHOST")))
                throw new Exception($"Cannot parse field RABBITMQ_VIRTUALHOST");

            Factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME"),
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT")),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
                VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VIRTUALHOST"),
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };
        }
    }
}