using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class CourseController : Controller
    {
		AttendanceEntities db = new AttendanceEntities();
		// GET: Course
		public ActionResult Index()
        {
			var courselist = db.Courses.ToList();
			return View(courselist);
		}
    }
}