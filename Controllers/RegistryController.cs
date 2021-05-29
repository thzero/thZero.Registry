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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using thZero.AspNetCore.Mvc;
using thZero.Registry.Requests;
using thZero.Responses;

namespace thZero.Registry.Controllers
{
    [ApiExplorerSettings(GroupName = Constants.ApiGroups.RegistryV1)]
    [ApiController]
    [Route("v1/api/[controller]")]
    public class RegistryController : BaseController<RegistryController>
    {
        public RegistryController(Services.Discovery.IDiscoveryService discoveryService, ILogger<RegistryController> logger) : base(logger)
        {
            _discoveryService = discoveryService;
        }

        [HttpDelete]
        public async Task<IActionResult> Deregister([FromQuery] RegistryRequest request)
        {
            var results = ModelState.IsValid;
            return JsonDelete(await _discoveryService.Deregister(Instrumentation, request));
        }

        [Route("listing")]
        [HttpPost]
        public async Task<IActionResult> Listing(ListingRegistryRequest request)
        {
            var results = ModelState.IsValid;
            return JsonPost(await _discoveryService.Listing(Instrumentation, request));
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterRegistryRequest request)
        {
            var results = ModelState.IsValid;
            return JsonPost(await _discoveryService.Register(Instrumentation, request));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RegistryRequest request)
        {
            var results = ModelState.IsValid;
            return JsonGet(await _discoveryService.Get(Instrumentation, request));
        }

        private readonly Services.Discovery.IDiscoveryService _discoveryService;
    }
}
