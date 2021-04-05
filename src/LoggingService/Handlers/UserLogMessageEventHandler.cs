﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoggingService.Models;
using LoggingService.Repositories;
using Microsoft.Extensions.Logging;

namespace LoggingService.Handlers
{
    public class UserLogMessageEventHandler : IntegrationEventHandler<IEnumerable<LogMessage>>
    {
        private readonly ILogger<UserLogMessageEventHandler> _logger;
        private readonly IRepository<LogMessage> _logMessageRepository = new MongoRepository<LogMessage>("UserLogMessage");

        public UserLogMessageEventHandler(ILogger<UserLogMessageEventHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task Handle(IEnumerable<LogMessage> msg)
        {
            await _logMessageRepository.CreateAll(msg);
        }

        protected override Task OnError(Exception ex, IEnumerable<LogMessage> msg)
        {
            _logger.LogError(ex, ex.Message);
            return Task.CompletedTask;
        }

        protected override Task Always(IEnumerable<LogMessage> msg) => Task.CompletedTask;

        protected override Task OnSuccess(IEnumerable<LogMessage> msg) => Task.CompletedTask;
    }
}