using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Extension
{
	public static class UrlHelperExtensions
	{
		/// <summary>
		/// Extension method to handle localized URL using a dedicated, multi-language custom route.
		/// for additional info, read the following post:
		/// https://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/
		/// </summary>
		public static string MultilanguageAction(
			this UrlHelper helper,
			string actionName,
			string controllerName,
			object routeValues,
			CultureInfo cultureInfo)
		{
			// fallback if cultureInfo is NULL
			if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

			// arrange a "localized" controllerName to be handled with a dedicated localization-aware route.
			string localizedControllerName = String.Format("{0}/{1}",
				cultureInfo.TwoLetterISOLanguageName, controllerName);

			// build the Action
			return helper.Action(
				actionName,
				localizedControllerName,
				routeValues);
		}
	}
}