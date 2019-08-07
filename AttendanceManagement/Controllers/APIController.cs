using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using AttendanceManagement.Models;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Collections;

namespace AttendanceManagement.Controllers
{
    public class APIController : ApiController
    {
		public string ReadData(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			Stream receiveStream = response.GetResponseStream();
			StreamReader readStream = null;
			readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
			string data = readStream.ReadToEnd().ToString();
			response.Close();
			readStream.Close();
			return data;
		}
	}

    public class JSONController : Controller
    {
        public JsonResult GetSemesters()
        {
            using (var db = new AttendanceEntities())
            {
                return Json(db.Courses.Where(c => c.Semester.HasValue).Select(c => c.Semester.Value).Distinct().OrderBy(s => s).ToArray(),
                            JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCourses(string email, int hk)
        {
            using (var db = new AttendanceEntities())
            {
                return Json(db.Courses.Where(c => c.Lecturer == email && c.Semester == hk).ToList().Select(c => new
                {
                    id = c.ID,
                    Name = c.CourseName,
                    Students = c.Students,
                    Day = c.DayOfWeek,
                    Time = c.TimeSpan.HasValue ? String.Format("{0}:{1}", c.TimeSpan.Value.Hours, c.TimeSpan.Value.Minutes) : "",
                    Room = c.Room
                }).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStudents(int courseID)
        {
            using (var db = new AttendanceEntities())
            {
                return Json(db.CourseMembers.Where(m => m.CourseID == courseID).ToList().Select(s => new
                {
                    Code = s.StudentID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Birthday = s.DoB.HasValue ? s.DoB.Value.ToString("yyyy-MM-dd") : ""
                }).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSessions(int courseID)
        {
            using (var db = new AttendanceEntities())
            {
                return Json(db.Sessions.Where(s => s.CourseID == courseID).ToList().Select(s => new
                {
                    id = s.ID,
                    Day = s.Date.HasValue ? s.Date.Value.ToString("yyyy-MM-dd") : "",
                    Time = s.Time.HasValue ? String.Format("{0}:{1}", s.Time.Value.Hours, s.Time.Value.Minutes) : ""
                }).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        private static readonly Hashtable Sessions = new Hashtable();
        private static readonly object padlock = new object();
        private static readonly Random Random = new Random();

        [System.Web.Mvc.Authorize]
        public ActionResult SetAttendance()
        {
            Session["User"] = User.Identity.Name;
            var random = Random.Next(1000, 9999);
            lock (padlock)
            {
                Sessions[random] = Session;
            }
            ViewBag.Random = random;
            return View("Attendance");
        }

        public void AddAttendance(int code, int courseID, int sessionID, string attendance)
        {
            var session = Sessions[code] as HttpSessionStateBase;
            if (session != null)
            {
                var email = session["User"];
                GlobalHost.ConnectionManager.GetHubContext<JSONHub>().Clients.All.addAttendance(email, attendance);
                lock (padlock)
                {
                    Sessions.Remove(code);
                }
            }
        }
    }

    [HubName("jsonHub")]
    public class JSONHub : Hub
    {
    }
}
