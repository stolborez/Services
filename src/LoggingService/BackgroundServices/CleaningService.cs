using System;
using System.Threading;
using System.Threading.Tasks;
using LoggingService.Models;
using LoggingService.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoggingService.BackgroundServices
{
    public class CleaningService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IRepository<LogMessage> _systemLogMessageRepository = new MongoRepository<LogMessage>("SystemLogMessage");
        private readonly IRepository<LogMessage> _userLogMessageRepository = new MongoRepository<LogMessage>("UserLogMessage");

        public CleaningService(ILogger<MessageReceiverService> logger)
        {
            _logger = logger;
        }

        private void CleanLog(object state)
        {
            _logger.LogInformation("CleanLog start.");
            try
            {
                _systemLogMessageRepository.DeleteManyAsync(_ => _.TimeStamp < DateTime.Now.AddMonths(-1)).Wait();
                _userLogMessageRepository.DeleteManyAsync(_ => _.TimeStamp < DateTime.Now.AddMonths(-1)).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CleaningService)} running.");
            bool success = Int32.TryParse(Environment.GetEnvironmentVariable("LOGGINGSERVICE_CLEANLOG_PERIOD"), out var  value);
            if (!success) value = 10;

            _timer = new Timer(CleanLog, null, TimeSpan.Zero, TimeSpan.FromSeconds(value));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}