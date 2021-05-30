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

using thZero.Instrumentation;
using thZero.Registry.Repository;
using thZero.Registry.Requests;
using thZero.Registry.Responses;
using thZero.Responses;
using thZero.Services;

namespace thZero.Registry.Services
{
    public sealed class RegistryService : ConfigServiceBase<RegistryService, Configuration.Application>, IRegistryService
    {
        public RegistryService(IRegistryRepository repository, IOptions<Configuration.Application> config, ILogger<RegistryService> logger) : base(config, logger)
        {
            _repository = repository;
        }

        #region Public Methods
        public async Task<SuccessResponse> CleanupAsync(IInstrumentationPacket packet)
        {
            Enforce.AgainstNull(() => packet);

            int cleanupInterval = Config.Registry?.Discovery?.HeartbeatInterval > 0 ? Config.Registry.Discovery.CleanupInterval : 45;
            return await _repository.CleanupAsync(packet, cleanupInterval);
        }

        public async Task<SuccessResponse> Deregister(IInstrumentationPacket packet, RegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);

            return await _repository.DeregisterAsync(packet, request);
        }

        public async Task<RegistrySuccessResponse> Get(IInstrumentationPacket packet, RegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);

            return await _repository.GetAsync(packet, request);
        }

        public async Task<ListingRegistrySuccessResponse> Listing(IInstrumentationPacket packet, ListingRegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);

            return await _repository.ListingAsync(packet, request);
        }

        public async Task<SuccessResponse> Register(IInstrumentationPacket packet, RegisterRegistryRequest request)
        {
            Enforce.AgainstNull(() => packet);

            return await _repository.RegistryAsync(packet, request);
        }
        #endregion

        #region Fields
        private readonly IRegistryRepository _repository;
        #endregion
    }
}
