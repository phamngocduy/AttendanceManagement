﻿using Microsoft.Owin.Security.Provider;

namespace Microsoft.Owin.Security.VanLang
{
    /// <summary>
    /// Context passed when a Challenge causes a redirect to authorize endpoint in the Microsoft account middleware
    /// </summary>
    public class VanLangApplyRedirectContext : BaseContext<VanLangAuthenticationOptions>
    {
        /// <summary>
        /// Creates a new context object.
        /// </summary>
        /// <param name="context">The OWIN request context</param>
        /// <param name="options">The Microsoft account middleware options</param>
        /// <param name="properties">The authenticaiton properties of the challenge</param>
        /// <param name="redirectUri">The initial redirect URI</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#",
            Justification = "Represents header value")]
        public VanLangApplyRedirectContext(IOwinContext context, VanLangAuthenticationOptions options,
            AuthenticationProperties properties, string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        /// <summary>
        /// Gets the URI used for the redirect operation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Represents header value")]
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authenticaiton properties of the challenge
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }
    }
}