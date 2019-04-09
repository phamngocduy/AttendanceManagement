using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
	public class UsersController : Controller
	{
		cap21t4Entities db = new cap21t4Entities();
		// GET: User
		public ActionResult Index()
		{
			var allUser = db.Users.ToList();
			return View(allUser);
		}

		public ActionResult UserRole()
		{
			var memberlist = db.Users.Where(x => x.AspNetUser.AspNetRoles.Count() > 0);
			List<User> users = memberlist.ToList();
			List<UsersRoleView> userRole = new List<UsersRoleView>();
			//1 nguoi 1 role
			//foreach (var item in users)
			//{
			//	UserRoleView u = new UserRoleView();
			//	u.Email = item.Email;
			//	u.FullName = item.FullName;
			//	u.Avatar = item.AvatarBase64;
			//	var uRole = (from role in db.AspNetRoles
			//				 from user in role.AspNetUsers.Where(x => x.Id == item.UserID)
			//				 select role).First();
			//	u.Role = uRole.Name;
			//	userRole.Add(u);
			//}


			//1 nguoi 2 role
			foreach (var item in users)
			{
				var uRole = (from role in db.AspNetRoles
							 from user in role.AspNetUsers.Where(x => x.Id == item.UserID)
							 select role);

				if (uRole.Count() > 1)
				{
					foreach (var role in uRole)
					{
						UsersRoleView u = new UsersRoleView();
						u.Email = item.Email;
						u.FullName = item.FullName;
						u.Avatar = item.AvatarBase64;
						u.Role = role.Name;
						userRole.Add(u);

					}

				}
				else
				{
					UsersRoleView u = new UsersRoleView();
					u.Email = item.Email;
					u.FullName = item.FullName;
					u.Avatar = item.AvatarBase64;
					u.Role = uRole.First().Name;
					userRole.Add(u);
				}
			}
			return View(userRole);
		}
		public ActionResult CreateNewRole()
		{
			return View();
		}
	}
}