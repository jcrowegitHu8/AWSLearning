using FeatureFlagApi5.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagApi5.Services
{
    public class JsonFileFeatureService : IFeatureRepository
    {
        public FeatureStoreModel GetAll()
        {
            var assembly = Assembly.GetAssembly(typeof(YamlFileFeatureService));
            var resourceStream = assembly.GetManifestResourceStream("FeatureFlagApi.DataStoreSamples.JsonFeatureFlagStore.json");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var jsonString = reader.ReadToEnd();


                //yml contains a string containing your YAML
                var result = JsonConvert.DeserializeObject<FeatureStoreModel>(jsonString);
                return result;
            }
        }
    }
}
