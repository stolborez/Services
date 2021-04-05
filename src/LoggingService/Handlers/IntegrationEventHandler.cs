using System;
using System.Threading.Tasks;

namespace LoggingService.Handlers
{
    public abstract class IntegrationEventHandler<TEvent> where TEvent : class
    {
        public async Task Handler(TEvent msg)
        {
            var isFailure = false;

            try
            {
                await Handle(msg);
            }
            catch (Exception exception)
            {
                isFailure = true;
                await OnError(exception, msg);
            }
            finally
            {
                if (!isFailure) await OnSuccess(msg);
                await Always(msg);
            }
        }

        protected abstract Task Handle(TEvent msg);
        protected abstract Task OnError(Exception ex, TEvent msg);
        protected abstract Task Always(TEvent msg);
        protected abstract Task OnSuccess(TEvent msg);
    }
}