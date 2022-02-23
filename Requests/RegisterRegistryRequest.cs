/* ------------------------------------------------------------------------- *
thZero.Registry
Copyright (C) 2021-2022 thZero.com

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

using thZero.Registry.Data;

namespace thZero.Registry.Requests
{
    public class RegisterRegistryRequest : RegistryData
    {
        public RegisterRegistryRequest()
        {
        }

        public RegisterRegistryRequest(RegistryData data)
        {
            Address = data.Address;
            Authentication = data.Authentication;
            Dns = data.Dns;
            Grpc = data.Grpc;
            HealthCheck = data.HealthCheck;
            Name = data.Name;
            Notes = data.Notes;
            Port = data.Port;
            Secure = data.Secure;
            Static = data.Static;
            Timestamp = data.Timestamp;
            Ttl = data.Ttl;
        }
    }
}
