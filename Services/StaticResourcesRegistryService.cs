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
using Microsoft.Extensions.Options;

using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Registry.Repository;
using thZero.Registry.Requests;
using thZero.Responses;
using thZero.Services;

namespace thZero.Registry.Services
{
    public sealed class StaticResourcesRegistryService : ConfigServiceBase<StaticResourcesRegistryService, Configuration.Application>, IStaticResourcesRegistryService
    {
        public StaticResourcesRegistryService(IRegistryRepository repository, IOptions<Configuration.Application> config, ILogger<StaticResourcesRegistryService> logger) : base(config, logger)
        {
            _repository = repository;
        }

        #region Public Methods
        public async Task<SuccessResponse> LoadAsync(IInstrumentationPacket packet)
        {
            ICollection<RegistryData> resources = Config.Registry?.Discovery?.Resources?.Count > 0 ? Config.Registry.Discovery.Resources : null;
            if (resources == null)
                return await Task.FromResult(Success());

            await resources.ToAsyncEnumerable().AsyncParallelForEach<RegistryData>(async entry =>
                    {
                        entry.Static = true;
                        await _repository.RegistryAsync(packet, new RegisterRegistryRequest(entry));
                    },
                    20,
                    TaskScheduler.Default
                );

            return await Task.FromResult(Success());
        }
        #endregion

        #region Fields
        private readonly IRegistryRepository _repository;
        #endregion
    }
}
