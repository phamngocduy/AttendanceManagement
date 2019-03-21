using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class AttendanceController : Controller
    {
        cap21t4Entities db = new cap21t4Entities();
        // GET: Attendance
        public ActionResult Index()
        {
            return View();
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
    }
}