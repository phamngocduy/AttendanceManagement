using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
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