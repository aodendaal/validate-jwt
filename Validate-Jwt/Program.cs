﻿using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace JwtCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = "";
            var tenantId = "";
            var rawIdToken = "";

            try
            {
                var user = Validate(clientId, tenantId, rawIdToken).GetAwaiter().GetResult();
                Display(user);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private static async Task<ClaimsPrincipal> Validate(string clientId, string tenantId, string token)
        {
            // ShowPII should be false to comply with GDPR
            IdentityModelEventSource.ShowPII = true;

            var auth0Domain = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            var auth0Audience = clientId;

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{auth0Domain}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = auth0Domain,
                ValidAudiences = new[] { auth0Audience },
                IssuerSigningKeys = openIdConfig.SigningKeys
            };

            SecurityToken validatedToken;
            var handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken(token, validationParameters, out validatedToken);

            return user;
        }

        private static void Display(ClaimsPrincipal user)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Token is validated. User Id {user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"{claim.Type} : {claim.Value}");
            }
            Console.ResetColor();
        }
    }
}
