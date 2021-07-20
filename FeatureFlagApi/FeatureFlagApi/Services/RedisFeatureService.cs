using FeatureFlag.Shared.Helper;
using FeatureFlagApi.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FeatureFlagApi.Services
{
    public class RedisFeatureService : IFeatureRepository
    {
        private readonly IDatabase _database;
        private readonly IConfiguration _configuration;
        private readonly string _FeatureStoreRedisKey;
        private readonly ILogger _logger;

        public RedisFeatureService(IDatabase database, 
            IConfiguration configuration, 
            ILogger logger)
        {
            _database = database;
            _configuration = configuration;
            var featureStoreRedisKey = _configuration.GetValue<string>("redis_featurestore_key");
            Guard.AgainstNull(featureStoreRedisKey, nameof(featureStoreRedisKey));
            _FeatureStoreRedisKey = featureStoreRedisKey;
            _logger = logger;
        }

        public FeatureStoreModel GetAll()
        {
            string ymlStringFromRedis = _database.StringGet(_FeatureStoreRedisKey);

            if (string.IsNullOrWhiteSpace(ymlStringFromRedis))
            {

                return FillRedisFromInMemory();

            }
            _logger.LogDebug("Redis had feature store values.");
            var deserializer = new DeserializerBuilder().Build();
            var result = deserializer.Deserialize<FeatureStoreModel>(ymlStringFromRedis);
            return result;

        }

        private FeatureStoreModel FillRedisFromInMemory()
        {
            _logger.LogDebug("Redis did not have feature store values.");

            var assembly = Assembly.GetAssembly(typeof(YamlFileFeatureService));
            var resourceStream = assembly.GetManifestResourceStream("FeatureFlagApi.DataStoreSamples.YamlFeatureFlagStore.yml");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var ymlString = reader.ReadToEnd();
                _logger.LogDebug("Redis now has feature store values.");
                _database.StringSet(_FeatureStoreRedisKey, ymlString);

                var deserializer = new DeserializerBuilder().Build();

                //yml contains a string containing your YAML
                var result = deserializer.Deserialize<FeatureStoreModel>(ymlString);
                return result;
            }
        }
    }
}
