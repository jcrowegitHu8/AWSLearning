using FeatureFlag.Shared.Helper;
using FeatureFlag.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FeatureFlagApi.SDK
{
    public interface IFeatureFlagService
    {
        bool FeatureIsOn(string featureName);
        bool FeatureIsOff(string featureName);
        Task<bool> FeatureIsOnAsync(string featureName, CancellationToken cancellationToken = default);
        Task<bool> FeatureIsOffAsync(string featureName, CancellationToken cancellationToken = default);
    }

    public class FeatureFlagService : IFeatureFlagService
    {
        private FeatureFlagSDKOptions _options;
        private readonly HttpClient client = new HttpClient();
        private const bool THIS_FEATURE_IS_OFF = false;
        private DateTime _nextRefreshTime = DateTime.MinValue;
        private readonly TimeSpan _minimumRefreshInterval = TimeSpan.FromMinutes(5);
        private static readonly ReaderWriterLockSlim _rwLockSlim = new ReaderWriterLockSlim();


        private EvaluationResponse _evaluationResponse;

        public FeatureFlagService(FeatureFlagSDKOptions options)
        {
            Guard.AgainstNull(options,nameof(options));
            Guard.AgainstNull(options.HttpClient,nameof(_options.HttpClient));

            _options = options;
            client = _options.HttpClient;
            if ( _options.RefreshInterval != null)
            {
                _minimumRefreshInterval = _options.RefreshInterval;
            }
        }


        public bool FeatureIsOn(string featureName)
        {
            return AsyncHelper.RunSync<bool>(() => FeatureIsOnAsync(featureName));
        }

        public bool FeatureIsOff(string featureName)
        {
            return !AsyncHelper.RunSync<bool>(() => FeatureIsOnAsync(featureName));
        }

        public async Task<bool> FeatureIsOffAsync(string featureName, CancellationToken cancellationToken = default)
        {
            return !await FeatureIsOnAsync(featureName, cancellationToken);
        }


        public async Task<bool> FeatureIsOnAsync(string featureName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                return THIS_FEATURE_IS_OFF;
            }

            await PopulateFeatureListAsync(cancellationToken);

            if (_evaluationResponse == null && _evaluationResponse.Features == null)
            {
                return THIS_FEATURE_IS_OFF;
            }

            return ThreadSafeSeachCollectionForFeatureState(featureName);
        }

        private bool ThreadSafeSeachCollectionForFeatureState(string featureName)
        {
            _rwLockSlim.EnterReadLock();
            try
            {
                var result = _evaluationResponse.Features
                    .FirstOrDefault(o => o.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase));
                if (result != null)
                {
                    return result.IsOn;
                }
            }
            finally
            {
                _rwLockSlim.ExitReadLock();
            }
            return THIS_FEATURE_IS_OFF;
        }



        private async Task PopulateFeatureListAsync(CancellationToken cancellationToken = default)
        {
            if (_evaluationResponse != null)
            {
                var isTimeToRefresh = _nextRefreshTime <= DateTime.UtcNow;
                if (!isTimeToRefresh)
                {
                    return;
                }
            }

            var request = new EvaluationRequest
            {
                Features = _options.FeaturesToTrack
            };
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/features", stringContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                _rwLockSlim.EnterWriteLock();
                try
                {
                    _evaluationResponse = JsonConvert.DeserializeObject<EvaluationResponse>(responseString);

                }
                finally
                {
                    _rwLockSlim.ExitWriteLock();
                }
            }
            _nextRefreshTime = DateTime.UtcNow.Add(_minimumRefreshInterval);
        }

    }

    public class FeatureFlagSDKOptions
    {
        public TimeSpan RefreshInterval { get; set; }

        public List<string> FeaturesToTrack { get; set; }
        public HttpClient HttpClient { get; set; }
    }
}
