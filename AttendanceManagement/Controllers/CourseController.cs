using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using Newtonsoft.Json;

namespace AttendanceManagement.Controllers
{
    public class CourseController : Controller
    {
		AttendanceEntities db = new AttendanceEntities();
		// GET: Course
		public ActionResult Index()
        {
			var listSemester = db.Courses.Select(x=>x.Semester).Distinct().OrderByDescending(x=>x.Value).ToList();
			ViewBag.Semester = listSemester;
			int semester= (int) listSemester.Max();
			ViewBag.SemesterSelected = semester;
			var courselist = db.Courses.Where(x => x.Semester == semester).ToList();
			return View(courselist);
		}
		[HttpPost]
		public ActionResult Index(string SemeterID)
		{
			int semester = int.Parse(SemeterID);
			var listSemester = db.Courses.Select(x => x.Semester).Distinct().OrderByDescending(x => x.Value).ToList();
			ViewBag.Semester = listSemester;
			ViewBag.SemesterSelected = semester;
			var courselist = db.Courses.Where(x=>x.Semester== semester).ToList();
			return View(courselist);
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
	}
}