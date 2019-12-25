using System;
using Microsoft.Owin.Security.VanLang;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="VanLangAuthenticationMiddleware"/>
    /// </summary>
    public static class VanLangAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using Microsoft Account
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
        /// Authenticate users using Microsoft Account
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="clientId">The application client ID assigned by the Microsoft authentication service</param>
        /// <param name="clientSecret">The application client secret assigned by the Microsoft authentication service</param>
        /// <returns></returns>
        public static IAppBuilder UseVanLangAuthentication(
            this IAppBuilder app,
            string clientId,
            string clientSecret)
        {
            return UseVanLangAuthentication(
                app,
                new VanLangAuthenticationOptions
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                });
        }
    }
}