using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureADB2C.WebApp.Shared
{
    public class AzureAdB2cOptions
    {

        public AzureAdB2cOptions()
        {
            AzureAdB2cInstance = $"https://login.microsoftonline.com/tfp";
        }

        public const string PolicyAuthenticationPolicy = "Policy";

        public string Tenant { get; set; }

        public string ClientID { get; set; }

        public string  AzureAdB2cInstance { get; set; }

        public string RedirectURI { get; set; }

        public string ClientSecret { get; set; }

        public string SignUpAndSignInPolicyId { get; set; }

        public string EditProfilePolicy { get; set; }

        public string ResetPasswordPolicy { get; set; }

        public string ApiUrl { get; set; }

        public string ApiScopes { get; set; }

        public string DefaultPolicy => SignUpAndSignInPolicyId;

        public string Authority => $"{AzureAdB2cInstance}/{Tenant}/{DefaultPolicy}/V2.0";


    }
}
