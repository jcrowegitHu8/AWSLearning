using FeatureFlag.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagApi.SDK
{
    public interface IFeatureFlagService
    {
        bool FeatureIsOn(string featureName);
    }

    public class FeatureFlagService : IFeatureFlagService
    {
        private FeatureFlagSDKOptions _options;
        private readonly HttpClient client = new HttpClient();
        private const bool THIS_FEATURE_IS_OFF = false;

        private EvaluationResponse _evaluationResponse;

        public FeatureFlagService(FeatureFlagSDKOptions options)
        {
            _options = options;
            client = new HttpClient();
        }

        /// <summary>
        /// Exposed for integration testing
        /// </summary>
        /// <param name="httpClient"></param>
        public FeatureFlagService(HttpClient httpClient, FeatureFlagSDKOptions options)
        {
            client = httpClient;
            _options = options;
        }


        public bool FeatureIsOn(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                return THIS_FEATURE_IS_OFF;
            }
            if (_evaluationResponse == null)
            {
                //TODO need to add thead locker
                var request = new EvaluationRequest
                {
                    Features = _options.FeaturesToTrack
                };
                var json = JsonConvert.SerializeObject(request);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                //TODO Fill the features;
                var postTask = Task.Run(() => client.PostAsync(_options.ApiUrl +"/api/features", stringContent));
                postTask.Wait();
                var response = postTask.Result;

                var readTask = Task.Run(() => response.Content.ReadAsStringAsync());
                readTask.Wait();
                var responseString = readTask.Result;
                _evaluationResponse = JsonConvert.DeserializeObject<EvaluationResponse>(responseString);
            }

            if (_evaluationResponse == null && _evaluationResponse.Features == null)
            {
                return THIS_FEATURE_IS_OFF;
            }
            var result = _evaluationResponse.Features.FirstOrDefault(o => o.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase));
            if (result != null)
            {
                return result.IsOn;
            }
            return THIS_FEATURE_IS_OFF;
        }

    }

    public class FeatureFlagSDKOptions
    {
        public int CacheTimeInSeconds { get; set; }
        public string ApiUrl { get; set; }
        public List<string> FeaturesToTrack { get; set; }
    }
}
