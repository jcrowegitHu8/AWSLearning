using FeatureFlag.Shared.Helper;
using FeatureFlag.Shared.Models;
using Microsoft.Extensions.Logging;
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
        private readonly HttpClient _client = new HttpClient();
        private const bool THIS_FEATURE_IS_OFF = false;
        private DateTime _nextRefreshTime = DateTime.MinValue;
        private readonly TimeSpan _minimumRefreshInterval = TimeSpan.FromSeconds(5);
        private static readonly ReaderWriterLockSlim _rwLockSlim = new ReaderWriterLockSlim();
        private readonly ILogger _logger;

        /// <summary>
        /// In the event there are no feature to be tracked
        /// all feature checks will return false.
        /// </summary>
        private readonly bool _doNothing = false;


        private EvaluationResponse _evaluationResponse;

        public FeatureFlagService(FeatureFlagSDKOptions options)
        {
            Guard.AgainstNull(options, nameof(options));
            Guard.AgainstNull(options.HttpClient, nameof(_options.HttpClient));
            Guard.AgainstNull(options.Logger, nameof(_options.Logger));

            _options = options;
            _logger = _options.Logger;
            _client = _options.HttpClient;
            _logger.LogDebug("{ThreadId} FeatureFlagApi: {url}", ThreadId, _client.BaseAddress);
            if (_options.RefreshInterval != null
                && _options.RefreshInterval >= _minimumRefreshInterval)
            {
                _logger.LogDebug("{ThreadId} Refresh interval is {seconds} seconds.", ThreadId, _options.RefreshInterval.TotalSeconds);
                _minimumRefreshInterval = _options.RefreshInterval;
            }
            else
            {
                _logger.LogDebug("{ThreadId} Refresh interval is the default 5 minutes", ThreadId);
                _minimumRefreshInterval = TimeSpan.FromMinutes(5);
            }

            if (_options.FeaturesToTrack == null || !_options.FeaturesToTrack.Any())
            {
                _doNothing = true;
                _logger.LogWarning("{ThreadId} There are no featues to track.  All feature checks will return OFF", ThreadId);
            }
            _logger.LogTrace("{ThreadId} Constructed successfully.", ThreadId);
        }

        private int ThreadId
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId;
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
            if (string.IsNullOrWhiteSpace(featureName) || _doNothing)
            {
                return THIS_FEATURE_IS_OFF;
            }

            await PopulateFeatureListAsync(cancellationToken);

            if (_evaluationResponse == null || _evaluationResponse.Features == null)
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
                _logger.LogTrace("{ThreadId} Reading Features.", ThreadId);
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
                _logger.LogTrace("{ThreadId} Checking Refresh Interval", ThreadId);
                var isTimeToRefresh = _nextRefreshTime <= DateTime.UtcNow;
                if (!isTimeToRefresh)
                {
                    return;
                }
            }

            _logger.LogTrace("{ThreadId} Fetching Data", ThreadId);
            var request = new EvaluationRequest
            {
                Features = _options.FeaturesToTrack
            };
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync("/api/features", stringContent, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _rwLockSlim.EnterWriteLock();
                    try
                    {
                        _logger.LogTrace("{ThreadId} Writing Data", ThreadId);
                        _evaluationResponse = JsonConvert.DeserializeObject<EvaluationResponse>(responseString);

                    }
                    finally
                    {
                        _nextRefreshTime = DateTime.UtcNow.Add(_minimumRefreshInterval);
                        _rwLockSlim.ExitWriteLock();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "{ThreadId} Unable to get feature list.", ThreadId);
            }
            _nextRefreshTime = DateTime.UtcNow.Add(_minimumRefreshInterval);
        }

    }

    public class FeatureFlagSDKOptions
    {
        public TimeSpan RefreshInterval { get; set; }

        public List<string> FeaturesToTrack { get; set; }
        public HttpClient HttpClient { get; set; }

        public ILogger Logger { get; set; }
    }
}
