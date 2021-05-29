﻿/* ------------------------------------------------------------------------- *
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Nito.AsyncEx;

using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Registry.Repository.Discovery;
using thZero.Registry.Requests;
using thZero.Registry.Responses;
using thZero.Responses;
using thZero.Services;

namespace thZero.Registry.Services.Discovery.HealthCheck
{
    public sealed class HealthCheckDiscoveryService : ConfigServiceBase<HealthCheckDiscoveryService, Configuration.Application>, IHealthCheckDiscoveryService
    {
        public HealthCheckDiscoveryService(IDiscoveryRepository repository, IOptions<Configuration.Application> config, IServiceProvider provider, ILogger<HealthCheckDiscoveryService> logger) : base(config, logger)
        {
            _repository = repository;

            Initialize(provider);
        }

        #region Public Methods
        public async Task<SuccessResponse> PerformAsync(IInstrumentationPacket packet)
        {
            Enforce.AgainstNull(() => packet);

            Logger.LogInformation("HEARTBEAT for HEALTHCHECK");

            ListingDiscoverySuccessResponse listResponse = await _repository.ListingAsync(packet, new ListingRegistryRequest()
            {
                HealthCheck = true
            });
            if (!IsSuccess(listResponse))
            {
                Logger.LogInformation("\t...no resources.");
                return await Task.FromResult(Error());
            }

            // Get the packet via service locator pattern; generally considered an anti-pattern.
            int cleanupInterval = Config.Registry?.Discovery?.CleanupInterval > 0 ? Config.Registry.Discovery.CleanupInterval : 45;

            using (await _mutex.LockAsync())
            {
                cleanupInterval *= 1000;

                long now = DateTime.Now.Millisecond;
                await listResponse.Data.Select(l => new Wrapper(packet, l)).ToAsyncEnumerable().AsyncParallelForEach<Wrapper>(Perform,
                    20,
                    TaskScheduler.Default
                );
            }

            return await _repository.CleanupAsync(packet, cleanupInterval);
        }
        #endregion

        #region Private Methods
        private void Initialize(IServiceProvider provider)
        {
            Enforce.AgainstNull(() => provider);

            if (Initialized)
                return;

            lock (Lock)
            {
                if (Initialized)
                    return;

                _services.Add(Constants.Services.Discovery.HealthCheck.ServiceType.Grpc, (IPerformHealthCheckDiscoveryService)GetService(provider, typeof(GrpcPerformHealthCheckDiscoveryService)));
                _services.Add(Constants.Services.Discovery.HealthCheck.ServiceType.Http, (IPerformHealthCheckDiscoveryService)GetService(provider, typeof(HttpPerformHealthCheckDiscoveryService)));

                Initialized = true;
            }
        }

        private async Task<SuccessResponse> Perform(Wrapper entry)
        {
            Enforce.AgainstNull(() => entry);

            if ((entry.Data.HealthCheck == null) || !entry.Data.HealthCheck.Enabled)
                return await Task.FromResult(Success());

            string type = (!String.IsNullOrEmpty(entry.Data.HealthCheck.Type) ? entry.Data.HealthCheck.Type : Constants.Services.Discovery.HealthCheck.ServiceType.Http).ToLower();
            if ((entry.Data.Grpc != null) && entry.Data.Grpc.Enabled)
                type = Constants.Services.Discovery.HealthCheck.ServiceType.Grpc;
            Logger.LogDebug("type: {type}", type);

            IPerformHealthCheckDiscoveryService service = _services[type];
            if (service == null)
                return await Task.FromResult(Error("Invalid healthcheck '{type}' service.", type));

            Logger.LogInformation("\tHealth check for '{date}' via '{type}", entry.Data.Name, type);

            SuccessResponse responsePerform = await service.Perform(entry.Packet, entry.Data);
            entry.Status = responsePerform.Success;

            Logger.LogInformation("\t...healthcheck for '{date}' ${ response.success ? 'succeeded' : 'failed'}.", entry.Data.Name, responsePerform.Success);

            // TODO
            //await _serviceMonitoring.Gauge(entry.Packet, 'discovery.registry.healthcheck', responsePerform.Success ? 1 : 0, null, { tag: entry.Data.Name });

            if (!IsSuccess(responsePerform))
            {
                // TODO
                //_notification(entry.Packet, entry.Data);
                return await Task.FromResult(responsePerform);
            }

            entry.Data.Timestamp = thZero.Utilities.DateTime.Timestamp;

            return await Task.FromResult(Success());
        }
        #endregion

        #region Fields
        private static bool Initialized = false;
        private static readonly object Lock = new();
        private readonly IDiscoveryRepository _repository;
        private readonly IDictionary<string, IPerformHealthCheckDiscoveryService> _services = new Dictionary<string, IPerformHealthCheckDiscoveryService>();
        private static readonly AsyncLock _mutex = new();
        #endregion
    }

    public class Wrapper
    {
        public Wrapper(IInstrumentationPacket packet, RegistryData data)
        {
            Data = data;
            Packet = packet;
        }

        #region Public Properties
        public RegistryData Data { get; set; }
        public IInstrumentationPacket Packet { get; set; }
        public bool Status { get; set; }
        #endregion
    }
}
