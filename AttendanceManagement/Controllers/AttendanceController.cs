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

		public ActionResult Index1()
		{
			return View();
		}

		//public ActionResult CreateClassView()
		//{
		//	return PartialView("CreateClassView");
		//}

		//[HttpPost]
		//public ActionResult CreateClass(Class Class)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		Class nClass = new Class();
		//		nClass.ClassName = Class.ClassName;
		//		nClass.StartDate = Class.StartDate;
		//		nClass.CreatedDate = DateTime.Now.Date;
		//		nClass.CreatedBy = User.Identity.Name;
		//		db.Classes.Add(nClass);
		//		db.SaveChanges();
		//		return RedirectToAction("manageClass");
		//	}
		//	return RedirectToAction("Index");

		//}

		//public ActionResult Edit(string id)
		//{
		//	var ClassID = int.Parse(id);
		//	var editClass = db.Classes.FirstOrDefault(x => x.ID == ClassID);
		//	return PartialView("EditClassView", editClass);
		//}

		//[HttpPost]
		//public ActionResult Edit(Class editClass)
		//{
		//	var eClass = db.Classes.FirstOrDefault(x => x.ID == editClass.ID);

		//	if (ModelState.IsValid)
		//	{
		//		eClass.StartDate = editClass.StartDate;
		//		eClass.Description = editClass.Description;
		//		eClass.ModifiedBy = User.Identity.Name;
		//		eClass.ModifiedDate = DateTime.Now.Date;
		//		db.SaveChanges();
		//	}
		//	return RedirectToAction("Index");

		//}
		public ActionResult manageClass(string id)
		{

			return View();
		}
		public ActionResult ManageStudent()
		{
			var user = db.CourseMembers.ToList();
			ViewBag.User = user;
			return View();
		}
		public ActionResult manageSession()
		{
			return View();
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
		public ActionResult DetailClass1(string id)
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
				ViewBag.date = date.Value.ToShortDateString();
			}
			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).ToList();
			ViewData["students"] = student;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.studentName = iten.Name;
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
						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
					listAttendance.Add(detail);
				}
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
		public ActionResult CourseList()
		{
			//APIController api = new APIController();
			//string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getCourses");
			//CourseModel course = JsonConvert.DeserializeObject<CourseModel>(data);
			var course = db.Courses.ToList();
			return View(course);
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
				List<UserModel> studentinfo = JsonConvert.DeserializeObject<List<UserModel>>(studentdata);
				var student = studentinfo.First();
				var StudentInCourse = db.CourseMembers.FirstOrDefault(x => x.StudentID == student.StID);
				if (StudentInCourse == null)
				{
					CourseMember newMember = new CourseMember();
					newMember.CourseID = int.Parse(Session["CourseID"].ToString());
					newMember.StudentID = student.StID;
					newMember.Name = student.FullName;
					newMember.Email = student.Email;
					newMember.DoB = student.DoB;
					newMember.Avatar = student.Avatar;
					db.CourseMembers.Add(newMember);
				}
			}
			db.SaveChanges();
			Session["tabactive"] = "tab2";

			return RedirectToAction("DetailClass1", "Attendance", new { id = Session["CourseID"] });
		}
		public ActionResult SynCourse()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://sodaubai.vanlanguni.edu.vn/API/getCourses");
			CourseModel course = JsonConvert.DeserializeObject<CourseModel>(data);
			foreach (var item in course.Courses)
			{
				var SynCourse = db.Courses.FirstOrDefault(x => x.Code == item.Code && x.Type1 == item.Type1 && x.Type2 == item.Type2 && x.Semester == course.Semester);
				if (SynCourse == null)
				{
					Course newCourse = new Course();
					newCourse.Code = item.Code;
					newCourse.CourseName = item.Name;
					newCourse.Type1 = item.Type1;
					newCourse.Type2 = item.Type2;
					newCourse.Major = db.Majors.FirstOrDefault(x => x.Code == item.Major).ID;
					newCourse.Credit = item.Credit;
					newCourse.Lecturer = item.Lecturer;
					newCourse.Students = item.Students;
					newCourse.DayOfWeek = item.DayOfWeek;
					newCourse.TimeSpan = item.TimeSpan;
					newCourse.Periods = item.Periods;
					newCourse.Room = item.Room;
					newCourse.Semester = course.Semester;
					db.Courses.Add(newCourse);
				}
				else
				{
					SynCourse.Code = item.Code;
					SynCourse.CourseName = item.Name;
					SynCourse.Type1 = item.Type1;
					SynCourse.Type2 = item.Type2;
					SynCourse.Major = db.Majors.FirstOrDefault(x => x.Code == item.Major).ID;
					SynCourse.Credit = item.Credit;
					SynCourse.Lecturer = item.Lecturer;
					SynCourse.Students = item.Students;
					SynCourse.DayOfWeek = item.DayOfWeek;
					SynCourse.TimeSpan = item.TimeSpan;
					SynCourse.Periods = item.Periods;
					SynCourse.Room = item.Room;
					SynCourse.Semester = course.Semester;
				}
			}
			db.SaveChanges();
			return RedirectToAction("Index");
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

			return RedirectToAction("DetailClass1", "Attendance", new { id = Session["CourseID"] });
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
				Session["tabactive"] = "tab1";
			}

			return Json("true");
		}
	}
}