/* ------------------------------------------------------------------------- *
thZero.Registry
Copyright (C) 2021-2021 thZero.com

<development [at] thzero [dot] com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 * ------------------------------------------------------------------------- */

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Responses;

namespace thZero.Registry.Services.Discovery.HealthCheck
{
    public sealed class HttpPerformHealthCheckDiscoveryService : PerformHealthCheckDiscoveryService<HttpPerformHealthCheckDiscoveryService>, IPerformHealthCheckDiscoveryService
    {
        public HttpPerformHealthCheckDiscoveryService(IOptions<Configuration.Application> config, ILogger<HttpPerformHealthCheckDiscoveryService> logger) : base(config, logger)
        {
            _client.Timeout = TimeSpan.FromSeconds(15);
            _client.DefaultRequestHeaders.Accept.Clear();
        }

        #region Public Methods
        public async Task<SuccessResponse> Perform(IInstrumentationPacket packet, RegistryData registry)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => registry);

            if (String.IsNullOrEmpty(registry.HealthCheck.Path))
            {
                Logger.LogWarning("No healthCheck url provided");
                return await Task.FromResult(Error("No health check url provided."));
            }

            ResourceRegistryResponse response = await Utilities.Resources.GenerateUriAsync(packet, registry);
            if (!IsSuccess(response))
                return await Task.FromResult(Error("No registry uri provided."));

            response.Uri.Path = registry.HealthCheck.Path;

            try
            {
                using (CancellationTokenSource cancelAfterDelay = new(TimeSpan.FromSeconds(Timeout)))
                {
                    HttpResponseMessage responseHttp = await _client.GetAsync(response.Uri.Uri, cancelAfterDelay.Token);
                    return await Task.FromResult(responseHttp.StatusCode == HttpStatusCode.OK ? Success() : Error());
                }
            }
            catch (TaskCanceledException tcex)
            {
                Logger.LogError(tcex, null);
                return await Task.FromResult(Error("timeout"));
            }
            catch (WebException wex)
            {
                Logger.LogError(wex, null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, null);
            }

            return await Task.FromResult(Error());
        }
        #endregion

        #region Fields
        private static readonly HttpClient _client = new();
        #endregion
    }

    public class ResourceRegistryResponse : SuccessResponse
    {
        #region Public Methods
        public RegistryData Registry { get; set; }
        public UriBuilder Uri { get; set; }
        #endregion
    }
}
