// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Owin;
using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Microsoft.Owin.Security.VanLang
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

        public static async Task<ExternalLoginInfo> GetExternalLoginInfoAsync2(this IAuthenticationManager manager)
        {
            var loginInfo = await manager.GetExternalLoginInfoAsync();
            return loginInfo ?? HttpContext.Current.Session[Constants.ExternalLoginInfo] as ExternalLoginInfo;
        }

        public static async Task<SignInStatus> ExternalSignInAsync2<TUser, TKey>(this SignInManager<TUser, TKey> manager, ExternalLoginInfo loginInfo, UserManager<TUser, TKey> UserManager)
            where TUser : class, IUser<TKey> where TKey : IEquatable<TKey>
        {
            var result = await manager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                    return result;
                default:
                    //var user = new ApplicationUser { UserName = loginInfo.Email, Email = loginInfo.Email };
                    var user = Activator.CreateInstance<TUser>();
                    user.GetType().GetProperty("UserName").SetValue(user, loginInfo.Email);
                    user.GetType().GetProperty("Email").SetValue(user, loginInfo.Email);
                    if ((await UserManager.CreateAsync(user)).Succeeded)
                    {
                        var id = (TKey)user.GetType().GetProperty("Id").GetValue(user);
                        if ((await UserManager.AddLoginAsync(id, loginInfo.Login)).Succeeded)
                        {
                            await manager.SignInAsync(user, false, false);
                            return SignInStatus.Success;
                        }
                    }
                    return SignInStatus.Failure;
            }
        }
    }
}
