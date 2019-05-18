using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
namespace AttendanceManagement.Controllers
{
    public class SessionController : Controller
    {
		AttendanceEntities db = new AttendanceEntities();
        // GET: Session
        public ActionResult Index()
        {
            return View();
        }
		[HttpPost]
		public ActionResult Create(SessionModel newSession)
		{
			int CourseID = int.Parse(Session["CourseID"].ToString());
			for (int i = 0; i < newSession.TotalWeek; i++)
			{
				Session nSession = new Session();
				nSession.CourseID = CourseID;
				nSession.Date = newSession.StartDate.Value.AddDays(i * 7);
				nSession.Time = newSession.Time;
				db.Sessions.Add(nSession);
			}
			db.SaveChanges();
			Session["tabactive"] = "tab3";

			return RedirectToAction("DetailClass", "Attendance",new { id = Session["CourseID"] });
		}
		[HttpGet]
		public ActionResult ChangeToSession()
		{
			Session["tabactive"] = "tab3";
			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		public ActionResult Edit(string id)
		{
			var SessionID = int.Parse(id);
			var sessionModel = db.Sessions.FirstOrDefault(x => x.ID == SessionID);
			return View(sessionModel);
		}

		[HttpPost]
		public ActionResult Edit(Session session)
		{
			var sessionEdit = db.Sessions.FirstOrDefault(x => x.ID == session.ID);
			sessionEdit.Date = session.Date;
			sessionEdit.Time = session.Time;
			session.Note = session.Note;
			db.SaveChanges();
			Session["tabactive"] = "tab3";
			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });

		}

		[HttpPost]
		public ActionResult Delete(string sessionID)
		{
			int id = int.Parse(sessionID);
			db.Attendances.RemoveRange(db.Attendances.Where(x => x.SessionID == id));
			db.Sessions.RemoveRange(db.Sessions.Where(x => x.ID == id));
			db.SaveChanges();
		
			Session["tabactive"] = "tab3";
			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });

		}

	}
}