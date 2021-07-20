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

using thZero.Registry.Data;

namespace thZero.Registry.Configuration
{
    public class RegistryConfiguration
    {
        #region Public Properties
        public RegistryDiscovery Discovery { get; set; } = new RegistryDiscovery();
        public RegistryHealthCheck HealthCheck { get; set; } = new RegistryHealthCheck();
        #endregion
    }

    public class RegistryDiscovery
    {
        #region Public Properties
        public int CleanupInterval { get; set; }
        public int HeartbeatInterval { get; set; }
        public ICollection<RegistryData> Resources { get; set; } = new List<RegistryData>();
        #endregion
    }

    public class RegistryHealthCheck
    {
        #region Public Properties
        public int CleanupInterval { get; set; }
        public int HeartbeatInterval { get; set; }
        public int Timeout { get; set; }
        #endregion
    }
}
