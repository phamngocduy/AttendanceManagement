using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{action}/{id}",
                defaults: new { controller = "API", id = RouteParameter.Optional }
            );
        }
    }
}
