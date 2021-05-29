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
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using thZero.AspNetCore;

namespace thZero.Registry
{
    public class SharedKeyAuthenticationHandler : AuthenticationHandler<SharedKeyAuthenticationOptions>
    {
        public SharedKeyAuthenticationHandler(IOptionsMonitor<SharedKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IOptions<Configuration.Application> config)
            : base(options, logger, encoder, clock)
        {
            _config = config.Value;
        }

        #region Protected Methods
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string Declaration = "HandleAuthenticateAsync";

            try
            {
                var authHeader = CheckParameter();
                Logger.LogDebug(Logger.LogFormat(Declaration, "authHeader", () => { return authHeader; }));
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, SharedKeyAuthenticationOptions.AuthenticationScheme)
                    };
                    if (_config.Authorization.Key.Equals(authHeader))
                        claims.Add(new Claim(ApiKeyAuthorizeAttribute.KeyPolicy, authHeader));
                    if (_config.Authorization.KeyAdmin.Equals(authHeader))
                    {
                        claims.Add(new Claim(ApiKeyAuthorizeAttribute.KeyPolicy, authHeader));
                        claims.Add(new Claim(AdminApiKeyAuthorizeAttribute.KeyPolicy, authHeader));
                    }

                    if (claims.Count == 0)
                    {
                        Logger.LogDebug(Logger.LogFormat(Declaration, "Authenticate: Failed, no claims."));
                        return Task.FromResult(AuthenticateResult.Fail("No apiKey."));
                    }

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SharedKeyAuthenticationOptions.AuthenticationScheme));
                    var ticket = new AuthenticationTicket(principal, SharedKeyAuthenticationOptions.AuthenticationScheme);

                    Logger.LogDebug(Logger.LogFormat(Declaration, "Authenticate: Success."));
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }

                Logger.LogDebug(Logger.LogFormat(Declaration, "Authenticate: Failed."));
                return Task.FromResult(AuthenticateResult.Fail("No apiKey."));
            }
            catch (Exception ex)
            {
                Logger.LogDebug(Logger.LogFormat(Declaration, "Authenticate: Failed.", ex));
                return Task.FromResult(AuthenticateResult.Fail("No apiKey."));
            }
        }
        #endregion

        #region Private Methods
        private string CheckParameter()
        {
            string result = null;
            if (Request.Headers.ContainsKey(KeyAuthorization))
                result = Request.Headers[KeyAuthorization];
            else if (Request.Headers.ContainsKey(KeyAuthorization2))
                result = Request.Headers[KeyAuthorization2];
            else if (Request.Headers.ContainsKey(KeyAuthorization3))
                result = Request.Headers[KeyAuthorization3];
            else if (Request.Headers.ContainsKey(KeyAuthorization4))
                result = Request.Headers[KeyAuthorization4];

            return result;
        }
        #endregion

        #region Fields
        private readonly Configuration.Application _config;
        #endregion

        #region Constants
        private const string KeyAuthorization = "x-api-key";
        private const string KeyAuthorization2 = "x-auth-key";
        private const string KeyAuthorization3 = "authorization";
        private const string KeyAuthorization4 = "Authorization";
        #endregion
    }

    public class SharedKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        #region Public Methods
        public override void Validate()
        {
            Console.WriteLine("");
        }
        #endregion

        #region Constants
        public const string AuthenticationScheme = "SharedKey";
        #endregion
    }

    public static class SharedKeyAuthenticationHandlerExtensions
    {
        #region Public Methods
        public static AuthenticationBuilder AddSharedSecret(this AuthenticationBuilder builder, IServiceCollection services)
        {
            return builder.AddScheme<SharedKeyAuthenticationOptions, SharedKeyAuthenticationHandler>(
                "SharedKey", // Name of scheme
                "SharedKey", // Display name of scheme
                options =>
                {
                    //var provider = services.BuildServiceProvider();
                    // Logger, ServiceUserRepository and SharedKeyAuthenticationProcess are all things that were injected into the custom authentication
                    // middleware in ASP.NET Core 1.1. This is now added to the options object instead.
                    //options.Logger = provider.GetService<global::Serilog.ILogger>();
                    //options.ServiceUserRepository = provider.GetService<IServiceUserRepository>();
                    //options.SharedKeyAuthenticationProcess = provider.GetService<ISharedKeyAuthenticationProcess>();
                });
        }
        #endregion
    }
}
