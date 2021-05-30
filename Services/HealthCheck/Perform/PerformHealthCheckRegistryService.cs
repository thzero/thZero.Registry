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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using thZero.Services;

namespace thZero.Registry.Services.HealthCheck
{
    public abstract class PerformHealthCheckRegistryService<TService> : ConfigServiceBase<TService, Configuration.Application>
    {
        public PerformHealthCheckRegistryService(IOptions<Configuration.Application> config, ILogger<TService> logger) : base(config, logger)
        {
        }

        #region Protected Properties
        protected int Timeout
        {
            get
            {
                int? timeout = Config?.Registry?.HealthCheck?.HeartbeatInterval;
                return timeout.HasValue && timeout.Value > 0 ? timeout.Value : 5;
            }
        }
        #endregion
    }
}
