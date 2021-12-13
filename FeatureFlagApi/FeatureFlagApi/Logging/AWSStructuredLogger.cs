using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureFlagApi.Logging
{
    public interface IAWSStructuredLogger
    {
        EnrichLogDTO LogMeta { get; set; }

        void EnrichWithCorrelationId(Guid id);
        void EnrichWithCorrelationId(string id);
        void LogDebug(string message);
        void LogError(Exception ex, string message);
        void LogError(string message);
        void LogInfo(string message);
        void LogWarn(string message);
    }

    public class AWSStructuredLogger : IAWSStructuredLogger
    {
        private readonly ILogger<AWSStructuredLogger> _logger;

        public EnrichLogDTO LogMeta { get; set; }

        public AWSStructuredLogger(ILogger<AWSStructuredLogger> logger)
        {
            _logger = logger;
            LogMeta = new EnrichLogDTO();
        }

        public void EnrichWithCorrelationId(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                LogMeta.CorrelationId = id;
                LogInfo("CorreationId has been attached to structure logger.");
            }
        }

        public void EnrichWithCorrelationId(Guid id)
        {
            if (id == Guid.Empty)
            {
                LogMeta.CorrelationId = Guid.NewGuid().ToString();
                LogError("Attaching a GUID.Empty is invalid. Generating a new GUID for this request.  Please find and fix the code not sending and Empty.Guid CorrelationId.");
                return;
            }
            LogMeta.CorrelationId = id.ToString();
        }

        public void LogInfo(string message)
        {
            var jsonString = AggregateAndSerialize(message);
            _logger.LogInformation(jsonString);
        }

        public void LogWarn(string message)
        {
            var jsonString = AggregateAndSerialize(message);
            _logger.LogWarning(jsonString);
        }

        public void LogError(Exception ex, string message)
        {
            var jsonString = AggregateAndSerialize(message);
            _logger.LogError(ex, jsonString);
        }

        public void LogError(string message)
        {
            var jsonString = AggregateAndSerialize(message);
            _logger.LogError(jsonString);
        }

        public void LogDebug(string message)
        {
            var jsonString = AggregateAndSerialize(message);
            _logger.LogDebug(jsonString);
        }

        private string AggregateAndSerialize(string message)
        {
            LogMeta.Message = message;
            var jsonString = Serialize();
            return jsonString;
        }

        private string Serialize()
        {
            var jsonString = JsonConvert.SerializeObject(LogMeta,
                Newtonsoft.Json.Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            return jsonString;
        }
    }

    public class EnrichLogDTO
    {
        public string CorrelationId { get; set; }
        public string Message { get; set; }
    }
}
