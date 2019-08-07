using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using Newtonsoft.Json;

namespace AttendanceManagement.Controllers
{
	[Authorize]
	public class CourseController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
		// GET: Course
		[AllowAnonymous]
		public void setSemester(int semester)
		{
			Session["Semester"] = semester;
		}
		[AllowAnonymous]
		public int getSemester(AttendanceEntities db)
		{
			var listSemester = db.Courses.Select(x => x.Semester).Distinct().OrderByDescending(x => x.Value).ToList();
			int semester = (int)listSemester.Max();
			return (int)(Session["Semester"] ?? semester);
		}

		public ActionResult Index()
		{
			var semester = this.getSemester(db);
			var courseList = db.Courses.Where(x => x.Semester == semester).ToList();

			var lectureCourse = courseList.Where(x => x.Lecturer == User.Identity.Name);
			if(lectureCourse.Count() > 0)
			{
				return View(lectureCourse);
			}
			else
			{
				var studentCourse = courseList.Where(c => c.CourseMembers.Count(m => m.Email == User.Identity.Name) > 0);
				return View("StudentCourse", studentCourse.Distinct().ToList());
			}
		}
		public ActionResult StaffsCourses()
		{
			var semester = this.getSemester(db);

			var courseList = db.Courses.Where(x => x.Semester == semester).ToList();

			List<Major> majorOfUser = new List<Major>();
			var majorList = db.Majors.ToList();

			APIController api = new APIController();
			string groupdata = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getAllGroupID?email=" + User.Identity.Name);
			List<GroupModel> group = JsonConvert.DeserializeObject<List<GroupModel>>(groupdata);
			foreach (var majorItem in majorList)
			{
				foreach (var groupItem in group)
				{
					if (groupItem.ID == majorItem.GroupID)
					{
						majorOfUser.Add(majorItem);
					}
				}
			}
			List<Course> staffList = new List<Course>();
			foreach (var item in majorOfUser)
			{
				var listCourse = courseList.Where(x => x.Major == item.ID);
				foreach (var course in listCourse)
				{
					staffList.Add(course);
				}
			}

			return View(staffList);
		}
		public ActionResult studentSynCourse()
		{
			SynCourse();
			return RedirectToAction("StudentCourse");
		}

		public ActionResult lecturerSynCourse()
		{
			SynCourse();
			return RedirectToAction("LecturerCourses");
		}

		public ActionResult staffSynCourse()
		{
			SynCourse();
			return RedirectToAction("StaffsCourses");
		}

		public void SynCourse()
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
		}
	}
}