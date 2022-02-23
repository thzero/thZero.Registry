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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using thZero.AspNetCore;
using thZero.AspNetCore.Mvc;

namespace thZero.Registry.Controllers
{
    public class DiagnosticController : BaseController<DiagnosticController>
    {
        public DiagnosticController(ILogger<DiagnosticController> logger) : base(logger)
        {
        }

        [AllowAnonymous]
        [Route("diagnostics/health")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Health([FromServices] ISteeltoeInstrumentationControllerExtension service)
        {
            const string Declaration = "Health";

            try
            {
                var results = service.GetHealth(HttpContext);
                return Json(results);

            }
            catch (Exception ex)
            {
                Logger?.LogError2(Declaration, ex);
            }

            return new StatusCodeResult((int)System.Net.HttpStatusCode.ServiceUnavailable);
        }

        [AllowAnonymous]
        [Route("diagnostics/info")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Info([FromServices] ISteeltoeInstrumentationControllerExtension service)
        {
            const string Declaration = "Info";

            try
            {
                var results = service.GetInfo(HttpContext);
                return Json(results);

            }
            catch (Exception ex)
            {
                Logger?.LogError2(Declaration, ex);
            }

            return new StatusCodeResult((int)System.Net.HttpStatusCode.ServiceUnavailable);
        }

#if DEBUG
        [AllowAnonymous]
        [Route("diagnostics/mappings")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Mappings([FromServices] ISteeltoeInstrumentationControllerExtension service)
        {
            const string Declaration = "Mappings";

            try
            {
                var results = service.GetMappings(HttpContext);
                return Json(results);

            }
            catch (Exception ex)
            {
                Logger?.LogError2(Declaration, ex);
            }

            return new StatusCodeResult((int)System.Net.HttpStatusCode.ServiceUnavailable);
        }
#endif

        //[Authorize]
        [AllowAnonymous]
        [Route("diagnostics/metrics")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Metrics([FromServices] ISteeltoeInstrumentationControllerExtension service)
        {
            const string Declaration = "Metrics";

            try
            {
                var results = service.GetMetrics(HttpContext);
                return Json(results);

            }
            catch (Exception ex)
            {
                Logger?.LogError2(Declaration, ex);
            }

            return new StatusCodeResult((int)System.Net.HttpStatusCode.ServiceUnavailable);
        }
    }
}
