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
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Nito.AsyncEx;

using thZero.Data.Repository;
using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Registry.Requests;
using thZero.Registry.Responses;
using thZero.Responses;

namespace thZero.Registry.Repository
{
    public class MemRegistryRepository : RepositoryBase<MemRegistryRepository>, IRegistryRepository
    {
        public MemRegistryRepository(ILogger<MemRegistryRepository> logger) : base(logger)
        {
        }

        public override void Initialize(IRepositoryConnectionConfiguration connectionConfiguration)
        {
        }

        #region Public Methods
        public async Task<SuccessResponse> CleanupAsync(IInstrumentationPacket packet, long cleanupInterval)
        {
            Enforce.AgainstNull(() => packet);

            using (await _mutex.LockAsync())
            {
                cleanupInterval *= 1000;

                Logger.LogInformation("HEARTBEAT for CLEANUP");
                long now = Utilities.DateTime.Timestamp;
                List<string> deletable = new();
                await _registry.Values.ToAsyncEnumerable().AsyncParallelForEach<RegistryData>(async entry =>
                    {
                        if (entry.Static)
                            return;

                        long delta = now - entry.Timestamp;
                        if (delta <= cleanupInterval)
                            return;

                        deletable.Add(entry.Name);
                    },
                    20,
                    TaskScheduler.Default
                );

                Parallel.ForEach(deletable, entry =>
                {
                    Logger.LogInformation("\tremove stale name: '%s'", entry);
                    _registry.Remove(entry);
                });
            }

            return Success();
        }

        public async Task<SuccessResponse> DeregisterAsync(IInstrumentationPacket packet, RegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => request);

            if (!_registry.ContainsKey(request.Name))
                return await Task.FromResult(Error());

            using (await _mutex.LockAsync()) 
            {
                if (!_registry.ContainsKey(request.Name))
                    return await Task.FromResult(Error());

                _registry.Remove(request.Name);
            }

            return await Task.FromResult(Success());
        }

        public async Task<RegistrySuccessResponse> GetAsync(IInstrumentationPacket packet, RegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => request);

            if (!_registry.ContainsKey(request.Name))
                return Error(new RegistrySuccessResponse());

            RegistrySuccessResponse response = new();
            response.Registry = _registry[request.Name];

            return await Task.FromResult(response);
        }

        public async Task<ListingRegistrySuccessResponse> ListingAsync(IInstrumentationPacket packet, ListingRegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => request);

            IEnumerable<RegistryData> values = _registry.Values;
            // TODO: filtering...
            if (request.HealthCheck ?? false)
                values = values.Where(l => (l.HealthCheck != null) && l.HealthCheck.Enabled);

            ListingRegistrySuccessResponse response = new()
            {
                Data = values.ToList()
            };
            return await Task.FromResult(response);
        }

        public async Task<SuccessResponse> RegistryAsync(IInstrumentationPacket packet, RegisterRegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => request);

            using (await _mutex.LockAsync())
            {
                request.Timestamp = Utilities.DateTime.Timestamp;
                _registry.Add(request.Name, request);
            }

            return new SuccessResponse();
        }
        #endregion

        #region Fields
        private readonly IDictionary<string, RegistryData> _registry = new Dictionary<string, RegistryData>();

        //private static readonly object Lock = new();
        private static readonly AsyncLock _mutex = new();
        #endregion
    }
}
