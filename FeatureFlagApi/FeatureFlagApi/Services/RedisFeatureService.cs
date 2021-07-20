using FeatureFlag.Shared.Helper;
using FeatureFlagApi.Model;
using Microsoft.Extensions.Configuration;
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

        public RedisFeatureService(IDatabase database, IConfiguration configuration)
        {
            _database = database;
            _configuration = configuration;
            var featureStoreRedisKey = _configuration.GetValue<string>("redis_featurestore_key");
            Guard.AgainstNull(featureStoreRedisKey, nameof(featureStoreRedisKey));
            _FeatureStoreRedisKey = featureStoreRedisKey;
        }

        public FeatureStoreModel GetAll()
        {
            string ymlStringFromRedis = _database.StringGet(_FeatureStoreRedisKey);
            if (string.IsNullOrWhiteSpace(ymlStringFromRedis))
            {

                return FillRedisFromInMemory();

            }

            var deserializer = new DeserializerBuilder().Build();
            var result = deserializer.Deserialize<FeatureStoreModel>(ymlStringFromRedis);
            return result;

        }

        private FeatureStoreModel FillRedisFromInMemory()
        {
            var assembly = Assembly.GetAssembly(typeof(YamlFileFeatureService));
            var resourceStream = assembly.GetManifestResourceStream("FeatureFlagApi.DataStoreSamples.YamlFeatureFlagStore.yml");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var ymlString = reader.ReadToEnd();
                _database.StringSet(_FeatureStoreRedisKey, ymlString);

                var deserializer = new DeserializerBuilder().Build();

                //yml contains a string containing your YAML
                var result = deserializer.Deserialize<FeatureStoreModel>(ymlString);
                return result;
            }
        }
    }
}
