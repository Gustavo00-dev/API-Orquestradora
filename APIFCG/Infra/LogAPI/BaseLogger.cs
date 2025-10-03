using APIFCG.Infra.LogAPI.Services;

namespace APIFCG.Infra.LogAPI
{
    public class BaseLogger<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICorrelationIdGenerator _correlationId;
        protected readonly IElasticLogger _elasticLogger;

        public BaseLogger(
            ILogger<T> logger,
            ICorrelationIdGenerator correlationId,
            IElasticLogger elasticLogger
        )
        {
            _logger = logger;
            _correlationId = correlationId;
            _elasticLogger = elasticLogger;
        }


        public virtual void LogInformation(string message, string index)
        {
            logElk(message, "Information", index);

            _logger.LogInformation($"[CorrelationId: {_correlationId.Get()}] {message}");
        }

        public virtual void LogError(string message, string index)
        {
            logElk(message, "Error", index);
            _logger.LogError($"[CorrelationId: {_correlationId.Get()}] {message}");
        }

        public virtual void LogWarning(string message, string index)
        {
            logElk(message, "Warning", index);
            _logger.LogWarning($"[CorrelationId: {_correlationId.Get()}] {message}");
        }

        public virtual void logElk(string message, string level, string index)
        {
            _elasticLogger.RegisterLogAsync(new Models.LogEntry
            {
                Level = level,
                Message = message,
                CorrelationId = _correlationId.Get(),
                Source = typeof(T).FullName,
            }, index).Wait();
        }
    }
}
