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

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using thZero.AspNetCore;

namespace thZero.Registry
{
    public sealed class Program : SelfHostingMvcProgram<Program, Startup>
    {

        #region Public Methods
        public static void Main(string[] args)
        {
            Start(args);
        }
        #endregion

        #region Protected Methods
        protected override IWebHostBuilder InitializeWebHostBuilder(IWebHostBuilder builder)
        {
            return
                base.InitializeWebHostBuilder(builder);
        }

        protected override void InitializeConfigurationBuilder(WebHostBuilderContext hostingContext, IConfigurationBuilder configurationBuilder)
        {
            var env = hostingContext.HostingEnvironment;
            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        protected override void InitializeConfigureLoggingBuilder(WebHostBuilderContext hostingContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
#if DEBUG
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
#else
            loggingBuilder.AddNLog(hostingContext);
#endif
            loggingBuilder.AddEventSourceLogger();
        }
        #endregion
    }
}
