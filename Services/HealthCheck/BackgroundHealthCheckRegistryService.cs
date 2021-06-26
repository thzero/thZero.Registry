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
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using thZero.AspNetCore.Services;
using thZero.Instrumentation;

namespace thZero.Registry.Services.HealthCheck
{
    public class BackgroundHealthCheckRegistryService : BackgroundService<BackgroundHealthCheckRegistryService>
    {
        public BackgroundHealthCheckRegistryService(IHealthCheckRegistryService service, IOptions<Configuration.Application> config, IServiceProvider provider, ILogger<BackgroundHealthCheckRegistryService> logger) : base(logger)
        {
            _config = config.Value;
            _provider = provider;
            _service = service;
        }

        #region Public Methods
        protected override async Task StartAsyncI(CancellationToken cancellationToken)
        {
            await _service.PerformAsync((IInstrumentationPacket)_provider?.GetService(typeof(IInstrumentationPacket)));
        }
        #endregion

        #region Fields
        private readonly Configuration.Application _config;
        private readonly IServiceProvider _provider;
        private readonly IHealthCheckRegistryService _service;
        #endregion
    }
}
