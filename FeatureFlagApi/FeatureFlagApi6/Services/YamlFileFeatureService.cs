using FeatureFlagApi6.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FeatureFlagApi6.Services
{
    public class YamlFileFeatureService : IFeatureRepository
    {
        public FeatureStoreModel GetAll()
        {
            var assembly = Assembly.GetAssembly(typeof(YamlFileFeatureService));
            var resourceStream = assembly.GetManifestResourceStream("FeatureFlagApi6.DataStoreSamples.YamlFeatureFlagStore.yml");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var ymlString = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder().Build();

                //yml contains a string containing your YAML
                var result = deserializer.Deserialize<FeatureStoreModel>(ymlString);
                return result;
            }
        }
    }

}
