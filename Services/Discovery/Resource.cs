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
using System.Threading;
using System.Threading.Tasks;

using Makaretu.Dns;

using thZero.Instrumentation;
using thZero.Registry.Data;
using thZero.Registry.Services.Discovery.HealthCheck;
using thZero.Responses;

namespace thZero.Registry.Services.Discovery.Utilities
{
    public class Resources
    {
        public static async Task<ResourceRegistryResponse> GenerateUriAsync(IInstrumentationPacket packet, RegistryData registry)
        {
            Enforce.AgainstNull(() => packet);
            Enforce.AgainstNull(() => registry);

            int? port = registry.Port;
            bool secure = registry.Secure && registry.Secure;

            string address = registry.Address;
            if (registry.Dns != null)
            {
                IList<string> temp = new List<string>
                {
                    registry.Dns.Label
                };
                //if (registry.Dns.Local)
                //{
                //    temp.Add("local");
                //    address = String.Join('.', temp);
                //    //IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync(address);
                //    //address = results[0].IPAddress;

                //    var query = new Message();
                //    query.Questions.Add(new Question { Name = address, Type = DnsType.ANY });

                //    using (var mdns = new MulticastService())
                //    {
                //        mdns.Start();
                //        var response = await mdns.ResolveAsync(query, (new CancellationTokenSource(2000)).Token);
                //        if ((response.Answers == null) || (response.Answers.Count == 0))
                //            return new ResourceRegistryResponse() { Success = false }; // TODO

                //        // Do something
                //        if (!(response.Answers[0] is AddressRecord))
                //            return new ResourceRegistryResponse() { Success = false }; // TODO

                //        AddressRecord record = (AddressRecord)response.Answers[0];
                //        address = record.Address.ToString();
                //    }
                //}
                //else
                {
                    if (!String.IsNullOrEmpty(registry.Dns.Namespace))
                        temp.Add(registry.Dns.Namespace);
                    if (registry.Dns.Local)
                        temp.Add("local");
                    address = String.Join('.', temp);
                }
            }

            registry.Authentication = registry.Authentication;
            if (registry.Authentication != null)
                registry.Authentication = new();

            //`http${secure? 's' : ''}://${address}${port ? `:${port}` : ''}`;
            UriBuilder builder = new();
            builder.Scheme = "http" + (secure ? "s" : String.Empty);
            builder.Host = address;
            if (port.HasValue)
                builder.Port = port.Value;
            //StringBuilder temp2 = new();
            //temp2.Append("http").Append(secure ? 's' : '').Append("//").Append(address).Append(port.HasValue ? ":" : "").Append(port.HasValue ? port.Value : "");

            return await Task.FromResult(new ResourceRegistryResponse()
            {
                Registry = registry,
                Uri = builder
            });
        }
    }
}
