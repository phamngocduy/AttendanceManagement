using System;

namespace Microsoft.Owin.Security.VanLang
{
    internal static class Constants
    {
        internal const string DefaultAuthenticationType = "Microsoft";

        internal const string AuthorizationEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        internal const string TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        internal const string UserInformationEndpoint = "https://graph.microsoft.com/v1.0/me";
    }
}
