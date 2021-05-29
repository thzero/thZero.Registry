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
using System.Threading.Tasks;

using thZero.Instrumentation;
using thZero.Registry.Requests;
using thZero.Registry.Responses;
using thZero.Responses;

namespace thZero.Registry.Repository.Discovery
{
    public interface IDiscoveryRepository
    {
        Task<SuccessResponse> CleanupAsync(IInstrumentationPacket packet, long cleanupInterval);
        Task<SuccessResponse> DeregisterAsync(IInstrumentationPacket packet, RegistryRequest request);

        Task<DiscoverySuccessResponse> GetAsync(IInstrumentationPacket packet, RegistryRequest request);

        Task<ListingDiscoverySuccessResponse> ListingAsync(IInstrumentationPacket packet, ListingRegistryRequest request);

        Task<SuccessResponse> RegistryAsync(IInstrumentationPacket packet, RegisterRegistryRequest request);
    }
}
