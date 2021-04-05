using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LoggingService.Extensions;
using LoggingService.Models;
using LoggingService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoggingService.Controllers
{
    /// <summary>
    /// SystemLogMessage
    /// </summary>
    [ApiController]
    [Route("api/{version:apiVersion}/[controller]")]
    public class SystemLogMessageController : ControllerBase
    {
        private readonly IRepository<LogMessage> _logMessageRepository = new MongoRepository<LogMessage>("SystemLogMessage");
        private readonly ILogger<SystemLogMessageController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public SystemLogMessageController(ILogger<SystemLogMessageController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Find systemLogMessage
        /// </summary>
        /// <param name="minDateTime"></param>
        /// <param name="maxDateTime"></param>
        /// <param name="dateTime"></param>
        /// <param name="sourceService"></param>
        /// <param name="severity"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find")]
        [ProducesResponseType(typeof(PagedList<LogMessage>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Find(
            DateTime? minDateTime,
            DateTime? maxDateTime,
            DateTime? dateTime,
            string sourceService,
            Severity? severity,
            int pageNumber =1,
            int pageSize = 15
        )
        {
            Expression<Func<LogMessage, bool>> mainExpression = (_) => true;

            if (minDateTime != null)
            {
                Expression<Func<LogMessage, bool>> filterExpression = (_) => _.TimeStamp >= minDateTime;
                mainExpression = filterExpression.AndAlso(mainExpression);
            }

            if (maxDateTime != null)
            {
                Expression<Func<LogMessage, bool>> filterExpression = (_) => _.TimeStamp <= maxDateTime;
                mainExpression = filterExpression.AndAlso(mainExpression);
            }

            if (dateTime != null)
            {
                Expression<Func<LogMessage, bool>> filterExpression = (_) => _.TimeStamp == dateTime;
                mainExpression = filterExpression.AndAlso(mainExpression);
            }

            if (sourceService != null)
            {
                Expression<Func<LogMessage, bool>> filterExpression = (_) => _.SourceService.Equals(severity);
                mainExpression = filterExpression.AndAlso(mainExpression);
            }

            if (severity != null)
            {
                Expression<Func<LogMessage, bool>> filterExpression = (_) => _.Severity == severity;
                mainExpression = filterExpression.AndAlso(mainExpression);
            }

            PagedList<LogMessage> result = _logMessageRepository.FilterBy(mainExpression, pageNumber, pageSize);
            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(result);
        }
    }
}
