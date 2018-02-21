using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Clients;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using AzureADB2C.WebApp.Models;

namespace AzureADB2C.WebApp.Shared
{
    public class OpenIdConnectOptionsSetup : IConfigureOptions<OpenIdConnectOptions>
    {
        public OpenIdConnectOptionsSetup(IOptions<AzureAdB2cOptions> b2cOptions)
        {
            AzureAdB2cOptions = b2cOptions.Value;
        }

        public AzureAdB2cOptions AzureAdB2cOptions { get; set; }

        public void Configure(OpenIdConnectOptions options)
        {
            
        }

        public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            var code = context.ProtocolMessage.Code;

            string SignedInUserID = context.Principal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            TokenCache localTokenCache = new MSALSessionCache(SignedInUserID, context.HttpContext).GetMSALCacheInstance();

            AuthenticationContext authContext = new AuthenticationContext(AzureAdB2cOptions.Authority, localTokenCache);

            ClientCredential ca = new ClientCredential(AzureAdB2cOptions.ClientID, AzureAdB2cOptions.ClientSecret);

            try
            {

                var result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new System.Uri(AzureAdB2cOptions.RedirectURI), ca, "");

                context.HandleCodeRedemption(result.AccessToken, result.IdToken);
            }
            catch { }

        }
    }
}
