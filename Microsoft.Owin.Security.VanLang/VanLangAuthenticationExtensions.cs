// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.VanLang;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="VanLangAuthenticationMiddleware"/>
    /// </summary>
    public static class VanLangAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using VanLang
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseVanLangAuthentication(this IAppBuilder app, VanLangAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(VanLangAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using VanLang
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="appId">The appId assigned by VanLang</param>
        /// <param name="appSecret">The appSecret assigned by VanLang</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseVanLangAuthentication(
            this IAppBuilder app, string baseUrl)
        {
            return UseVanLangAuthentication(
                app, new VanLangAuthenticationOptions(baseUrl));
        }
    }
}
