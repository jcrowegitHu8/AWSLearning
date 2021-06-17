using FeatureFlagApi.Model;
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
    public class YamlFileFeatureService : IFeatureRepository
    {
        public List<Feature> GetAll()
        {
            var assembly = Assembly.GetAssembly(typeof(YamlFileFeatureService));
            var resourceStream = assembly.GetManifestResourceStream("FeatureFlagApi.DataStoreSamples.YamlFeatureFlagStore.yml");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var ymlString = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder().Build();

                //yml contains a string containing your YAML
                var result = deserializer.Deserialize<List<Feature>>(ymlString);
                return result;
            }
        }
    }

}
