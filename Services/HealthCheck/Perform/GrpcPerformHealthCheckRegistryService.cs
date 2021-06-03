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
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Grpc.Net.Client;

using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Responses;
using thZero.Services;
using System.Threading;

namespace thZero.Registry.Services.HealthCheck
{
    public sealed class GrpcPerformHealthCheckRegistryService : PerformHealthCheckRegistryService<GrpcPerformHealthCheckRegistryService>, IPerformHealthCheckRegistryService
    {
        public GrpcPerformHealthCheckRegistryService(IOptions<Configuration.Application> config, ILogger<GrpcPerformHealthCheckRegistryService> logger) : base(config, logger)
        {
        }

        #region Public Methods
        public async Task<SuccessResponse> Perform(IInstrumentationPacket packet, RegistryData registry)
        {
            const string Declaration = "Perform";

            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => registry);

            if (!registry.HealthCheck.Enabled)
            {
                Logger.LogWarning("No healthCheck url provided");
                return await Task.FromResult(Error("No health check url provided."));
            }

            ResourceRegistryResponse response = await Utilities.Resources.GenerateUriAsync(packet, registry);
            if (!IsSuccess(response))
                return await Task.FromResult(Error("No registry uri provided."));

            try
            {
                var channel = GrpcChannel.ForAddress(response.Uri.ToString());
                var client = new HealthCheck.HealthCheckClient(channel);

                using (CancellationTokenSource cancelAfterDelay = new(TimeSpan.FromSeconds(Timeout)))
                {
                    HealthCheckResponse responseClient = await client.PerformAsync(new HealthCheckRequest(), cancellationToken: cancelAfterDelay.Token);
                    return await Task.FromResult(responseClient.Success ? Success() : Error());
                }
            }
            catch (TaskCanceledException tcex)
            {
                Logger.LogError2(Declaration, tcex);
                return await Task.FromResult(Error("timeout"));
            }
            catch (Exception ex)
            {
                Logger.LogError2(Declaration, ex);
            }

            return await Task.FromResult(Error());
        }
        #endregion
    }
}
