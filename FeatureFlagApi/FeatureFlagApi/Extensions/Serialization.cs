using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FeatureFlagApi.Extensions
{
    public static class Serialization
    {
        public static void DumpAsJson(this object data)
        {
            Console.WriteLine("***Dumping Object Using JSON Serializer***");
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine(json);
        }

        public static void DumpAsYaml(this object data)
        {
            Console.WriteLine("***Dumping Object Using Yaml Serializer***");
            var stringBuilder = new StringBuilder();
            var serializer = new Serializer();
            stringBuilder.AppendLine(serializer.Serialize(data));
            var result = stringBuilder.ToString();
            Console.WriteLine(result);
        }
    }
}
