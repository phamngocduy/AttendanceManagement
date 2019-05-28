using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoginManagement
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				 name: "DefaultLocalized",
				 url: "{lang}/{controller}/{action}/{id}",
				 constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },   // en or en-US
				 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			 );
			routes.MapRoute(
			  name: "Default",
			  url: "LoginManagement/{controller}/{action}/{id}",
			  defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
		  );
			routes.MapRoute(
						  name: "Default1",
						  url: "{controller}/{action}/{id}",
						  defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
					  );
		}
	}
}
