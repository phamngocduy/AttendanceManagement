using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
	[Authorize]
    public class AttendanceController : Controller
    {
        cap21t4Entities db = new cap21t4Entities();
        // GET: Attendance
        public ActionResult Index()
        {
			var classlist = db.Classes.ToList();
            return View(classlist);
        }

		public ActionResult CreateClassView()
		{
			return PartialView("CreateClassView");
		}
		
		[HttpPost]
		public ActionResult CreateClass(Class Class)
		{
			if (ModelState.IsValid)
			{
				Class nClass = new Class();
				nClass.ClassName = Class.ClassName;
				nClass.StartDate = Class.StartDate;
				nClass.CreatedDate = DateTime.Now.Date;
				nClass.CreatedBy = User.Identity.Name;
				db.Classes.Add(nClass);
				db.SaveChanges();
				return RedirectToAction("manageClass");
			}
			return RedirectToAction("Index");

		}

        public ActionResult manageClass()
        {
            return View();
        }
        public ActionResult ManageStudent()
        {
            var user = db.Users.ToList();
            ViewBag.User = user;
            return View();
        }
        public ActionResult manageSession()
        {
            return View();
        }
    }
}