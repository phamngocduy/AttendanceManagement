// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.Security.VanLang
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class VanLangAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="VanLangAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">VanLang Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public VanLangAuthenticatedContext(IOwinContext context)
            : base(context)
        {
            //User = user;
            //AccessToken = accessToken;

            //Id = TryGetValue(user, "id");
            //Name = TryGetValue(user, "name");
            //Link = TryGetValue(user, "link");
            //UserName = TryGetValue(user, "username");
            //Email = TryGetValue(user, "email");
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        public JObject User1 { get; private set; }

        /// <summary>
        /// Gets the VanLang access token
        /// </summary>
        public string AccessToken1 { get; private set; }

        /// <summary>
        /// Gets the VanLang access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn1 { get; set; }

        /// <summary>
        /// Gets the VanLang user ID
        /// </summary>
        public string Id1 { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name1 { get; private set; }

        public string Link1 { get; private set; }

        /// <summary>
        /// Gets the VanLang username
        /// </summary>
        public string UserName1 { get; private set; }

        /// <summary>
        /// Gets the VanLang email
        /// </summary>
        public string Email1 { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}
