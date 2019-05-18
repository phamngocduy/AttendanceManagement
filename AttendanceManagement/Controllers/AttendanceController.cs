using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using System.Text;
using System.Transactions;

namespace AttendanceManagement.Controllers
{
	public class AttendanceController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
		APIController api = new APIController();

		// GET: Attendance
		public ActionResult Index()
		{
			var courselist = db.Courses.ToList();
			return View(courselist);

		}
		public ActionResult CreateFaculty()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateFaculty([Bind(Include = "Id,FacultyName,Description")] Faculty faculty)
		{
			if (ModelState.IsValid)
			{
				db.Faculties.Add(faculty);
				db.SaveChanges();
				return RedirectToAction("ManageFaculty");
			}

			return View(faculty);
		}

		public ActionResult ManageFaculty()
		{
			return View();
		}

		public ActionResult DetailFaculty()
		{
			return View();
		}

		public ActionResult EditClass1()
		{
			return View();
		}

		public ActionResult CreateClass1()
		{
			return View();
		}
		public ActionResult DetailClass(string id)
		{
			Session["CourseID"] = id;
			int courseID = int.Parse(id);
			ViewBag.tab = "tab1";
			if (Session["tabactive"] != null)
			{
				ViewBag.tab = Session["tabactive"].ToString();
				Session.Remove("tabactive");
				
			}
			if (Session["SessionID"] != null)
			{
				int sessionid = int.Parse(Session["SessionID"].ToString());
				DateTime? date = db.Sessions.FirstOrDefault(x => x.ID == sessionid).Date;
				TempData["date"] = date.Value.ToShortDateString();
				Session.Remove("SessionID");
			}
			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x=>x.Name).ToList();
			ViewData["students"] = student;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.studentName = iten.Name;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				List<DetailAttendance> listAttendance = new List<DetailAttendance>();
				foreach (var item in session)
				{

					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime) item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance == null)
					{
						detail.Status = "N/A";
						detail.Note = "";
					}
					else
					{
						detail.Status = attendance.Status;
						
						if (attendance.Status != "0")
						{
							TotalPresent++;
						}
						TotalPoint = TotalPoint + int.Parse(attendance.Status);
						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
					listAttendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}

			ViewData["attendance"] = list;
			return View();
		}
		public ActionResult MajorList()
		{
			var major = db.Majors.ToList();
			return View(major);
		}
		public void SynMajor()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getMajors");
			List<MajorsModel> major = JsonConvert.DeserializeObject<List<MajorsModel>>(data);
			foreach (var item in major)
			{
				if (db.Majors.FirstOrDefault(x => x.Code == item.code) == null)
				{
					Major newMajor = new Major();
					newMajor.Code = item.code;
					newMajor.Name = item.name;
					db.Majors.Add(newMajor);
					db.SaveChanges();
				}
			}
		}
		public ActionResult AddStudent(string groupname)
		{
			string groupdata = api.ReadData("http://localhost:54325/api/getAllGroups");
			List<GroupModel> group = JsonConvert.DeserializeObject<List<GroupModel>>(groupdata);
			ViewBag.Group = group;
			if (groupname != null)
			{
				string userdata = api.ReadData("http://localhost:54325/api/getMember?groupname=" + groupname);
				List<UserModel> user = JsonConvert.DeserializeObject<List<UserModel>>(userdata);
				return View(user);
			}

			return View();

		}
		[HttpPost]
		public ActionResult AddStudent(List<string> studentlist)
		{
			foreach (var item in studentlist)
			{
				string studentdata = api.ReadData("http://localhost:54325/api/getUserInfo?searchString=" + item);
				UserModel student = JsonConvert.DeserializeObject<UserModel>(studentdata);
				var courseID = int.Parse(Session["CourseID"].ToString());
				var course = db.Courses.FirstOrDefault(x => x.ID == courseID);
				var StudentInCourse = course.CourseMembers.FirstOrDefault(x => x.StudentID == student.StID);
				if (StudentInCourse == null)
				{
					CourseMember newMember = new CourseMember();
					newMember.CourseID = courseID;
					newMember.StudentID = student.StID;
					newMember.Name = student.FullName;
					newMember.Email = student.Email;
					newMember.DoB = student.DoB;
					newMember.Avatar = student.Avatar;
					course.CourseMembers.Add(newMember);
				}
			}
			db.SaveChanges();
			Session["tabactive"] = "tab2";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		public ActionResult FacultyIndex()
		{
			var faculty = db.Faculties.ToList();
			List<FacultyViewModel> listfaculty = new List<FacultyViewModel>();
			foreach (var item in faculty)
			{
				FacultyViewModel facultyView = new FacultyViewModel();
				facultyView.ID = item.ID;
				facultyView.Name = item.Name;
				facultyView.Description = item.Description;
				//facultyView.GroupLink = groupdb.Groups.FirstOrDefault(x => x.ID == item.GroupID).GroupName;
				facultyView.Majors = item.Majors;
				listfaculty.Add(facultyView);
			}
			return View(listfaculty);
		}
		[HttpGet]
		public ActionResult CreateFaculty1()
		{
			//	ViewBag.GroupMap = groupdb.Groups.Where(x => x.GroupParent != null).ToList();
			return View();
		}
		[HttpPost]
		public ActionResult CreateFaculty1(Faculty faculty)
		{
			Faculty newfaculty = new Faculty();
			newfaculty.Name = faculty.Name;
			newfaculty.Description = faculty.Description;
			newfaculty.GroupID = faculty.GroupID;
			db.Faculties.Add(newfaculty);
			db.SaveChanges();
			return RedirectToAction("FacultyIndex");
		}
		[HttpGet]
		public ActionResult CheckAttendance(string id)
		{
			Session["SessionID"] = id;
			Session["tabactive"] = "tab4";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		} 

		[HttpPost]
		public JsonResult CheckAttendance(List<AttendanceModel> attendance)
		{
			using (var scope = new TransactionScope())
			{
				int sessionID = int.Parse(Session["SessionID"].ToString());
				foreach (var item in attendance)
				{
					Attendance att = new Attendance();
					att.MemberID = item.memberID;
					att.SessionID = sessionID;
					att.Status = item.status;
					att.Note = item.note;
					db.Attendances.Add(att);
				}
				db.SaveChanges();
				scope.Complete();
				TempData.Remove("date");
				Session.Remove("SessionID");
				Session["tabactive"] = "tab1";
			}

			return Json("true");
		}
	}
}