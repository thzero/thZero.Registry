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
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using thZero.Instrumentation;

namespace thZero.Registry.Services.HealthCheck
{
    public class BackgroundHealthCheckRegistryService : IHostedService, IDisposable
    {
        public BackgroundHealthCheckRegistryService(IHealthCheckRegistryService service, IOptions<Configuration.Application> config, IServiceProvider provider, ILogger<BackgroundHealthCheckRegistryService> logger)
        {
            _config = config.Value;
            _logger = logger;
            _provider = provider;
            _service = service;
        }

        #region Public Methods
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            const string Declaration = "StartAsync";

            int heartbeatInterval = _config.Registry?.HealthCheck?.HeartbeatInterval > 0 ? _config.Registry.HealthCheck.HeartbeatInterval : 45;
            _timer = new Timer(o => {
                Task.Run(async () => {
                    try
                    {
                        await _service.PerformAsync((IInstrumentationPacket)_provider?.GetService(typeof(IInstrumentationPacket)));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError2(Declaration, ex);
                    }
                }).Wait(cancellationToken);
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(heartbeatInterval));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Fields
        private readonly Configuration.Application _config;
        private readonly ILogger<BackgroundHealthCheckRegistryService> _logger;
        private readonly IServiceProvider _provider;
        private readonly IHealthCheckRegistryService _service;
        private Timer _timer;
        #endregion
    }
}
