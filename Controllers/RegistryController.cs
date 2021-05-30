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

using thZero.AspNetCore;
using thZero.AspNetCore.Mvc;
using thZero.Registry.Requests;
using thZero.Utilities;

namespace thZero.Registry.Controllers
{
    [ApiExplorerSettings(GroupName = Constants.ApiGroups.RegistryV1)]
    [ApiController]
    [Route("v1/api/[controller]")]
    public class RegistryController : BaseController<RegistryController>
    {
        public RegistryController(Services.IRegistryService discoveryService, ILogger<RegistryController> logger) : base(logger)
        {
            _discoveryService = discoveryService;
        }

        [HttpDelete]
        public async Task<IActionResult> Deregister([FromQuery] RegistryRequest request)
        {
            const string Declaration = "Deregister";

            StopwatchTiming duration = null;
            try
            {
                duration = Stopwatch.Start(Declaration);

                await InitializeJsonPostResultAsync(request, null, async (model) =>
                {
                    return JsonDelete(await _discoveryService.Deregister(Instrumentation, request));
                });
            }
            catch (Exception ex)
            {
                Logger?.LogError(Declaration, ex);
            }
            finally
            {
                Stopwatch.StopLog(duration);
            }

            return JsonDelete(Error());
        }

        [Route("listing")]
        [HttpPost]
        public async Task<IActionResult> Listing(ListingRegistryRequest request)
        {
            const string Declaration = "Listing";

            StopwatchTiming duration = null;
            try
            {
                duration = Stopwatch.Start(Declaration);

                await InitializeJsonPostResultAsync(request, null, async (model) =>
                {
                    return JsonPost(await _discoveryService.Listing(Instrumentation, request));
                });
            }
            catch (Exception ex)
            {
                Logger?.LogError(Declaration, ex);
            }
            finally
            {
                Stopwatch.StopLog(duration);
            }

            return JsonPost(Error());
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterRegistryRequest request)
        {
            const string Declaration = "Register";

            StopwatchTiming duration = null;
            try
            {
                duration = Stopwatch.Start(Declaration);

                await InitializeJsonPostResultAsync(request, null, async (model) =>
                {
                    return JsonPost(await _discoveryService.Register(Instrumentation, request));
                });
            }
            catch (Exception ex)
            {
                Logger?.LogError(Declaration, ex);
            }
            finally
            {
                Stopwatch.StopLog(duration);
            }

            return JsonPost(Error());
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RegistryRequest request)
        {
            const string Declaration = "Get";

            StopwatchTiming duration = null;
            try
            {
                duration = Stopwatch.Start(Declaration);

                await InitializeJsonGetResultAsync(request, null, async (model) =>
                {
                    return JsonGet(await _discoveryService.Get(Instrumentation, request));
                });
            }
            catch (Exception ex)
            {
                Logger?.LogError(Declaration, ex);
            }
            finally
            {
                Stopwatch.StopLog(duration);
            }

            return JsonGet(Error());
        }

        private readonly Services.IRegistryService _discoveryService;
    }
}
