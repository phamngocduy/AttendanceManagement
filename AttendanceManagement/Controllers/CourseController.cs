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
		APIController api = new APIController();
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
			var allCourses = db.Courses.ToList();
			var courseList = allCourses.Where(x => x.Semester == semester).ToList();
			var allLecturerCourses = allCourses.Where(x => x.Lecturer == User.Identity.Name);
			if (allLecturerCourses.Count() > 0)
			{
				var lectureCourse = courseList.Where(x => x.Lecturer == User.Identity.Name);
				return View(lectureCourse);
			}
			else
			{
				// process data for student course

				var studentCourse = (from course in courseList
									 join courseMember in db.CourseMembers on course.ID equals courseMember.CourseID
									 where course.ID == courseMember.CourseID
									 where courseMember.Email == User.Identity.Name
									 select new { course, courseMember }).Distinct().ToList();


				if (studentCourse.Count() > 0)
				{
					List<studentCourseView> studentCourseModel = new List<studentCourseView>();
					foreach (var item in studentCourse)
					{
						studentCourseView studentView = new studentCourseView();
						studentView.studentName = item.courseMember.LastName + " " + item.courseMember.FirstName;
						studentView.studentID = item.courseMember.StudentID;
						studentView.studentDoB = (DateTime)item.courseMember.DoB;
						studentView.courseName = item.course.CourseName;
						studentView.courseID = item.course.ID.ToString();
						var iAttendanceCount = 0;
						var iAttendancePointCount = 0;
						var sessionList = db.Sessions.Where(x => x.CourseID == item.course.ID);
						studentView.totalSession = sessionList.Count();
						foreach (var session in sessionList)
						{
							var attendanceDetail = new DetailAttendance();
							var attendanceInformation = session.Attendances.FirstOrDefault(x => x.MemberID == item.courseMember.ID);
							if (attendanceInformation != null)
							{
								attendanceDetail.Date = (DateTime)session.Date;
								attendanceDetail.Note = attendanceInformation.Note;
								attendanceDetail.Status = attendanceInformation.Status;
								if (attendanceDetail.Status != "0" && attendanceDetail.Status != null)
								{
									iAttendanceCount++;
									iAttendancePointCount = iAttendancePointCount + int.Parse(attendanceDetail.Status);
								}
							}
							studentView.attendanceCount = iAttendanceCount;
							studentView.attendancePoint = iAttendancePointCount;
							studentView.attendanceList.Add(attendanceDetail);
						}
						studentCourseModel.Add(studentView);
					}
					return View("StudentCourse", studentCourseModel);
				}
				else
				{
					var studentInfo = db.CourseMembers.FirstOrDefault(x => x.Email == User.Identity.Name);
					List<studentCourseView> studentCourseModel = new List<studentCourseView>();
					studentCourseView studentCourseItem = new studentCourseView();

					studentCourseItem.studentName = studentInfo.LastName + " " + studentInfo.FirstName;
					studentCourseItem.studentID = studentInfo.StudentID;
					studentCourseItem.studentDoB = (DateTime)studentInfo.DoB;

					studentCourseModel.Add(studentCourseItem);

					return View("StudentCourse", studentCourseModel);
				}


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
			SynMajor();
			SynCourse();
			return RedirectToAction("StudentCourse");
		}

		public ActionResult lecturerSynCourse()
		{
			SynMajor();
			SynCourse();
			return RedirectToAction("LecturerCourses");
		}

		public ActionResult staffSynCourse()
		{
			SynMajor();
			SynCourse();
			return RedirectToAction("StaffsCourses");
		}

		public void SynMajor()
		{
			var majors = api.getMajors();
			foreach (var item in majors)
			{
				if (db.Majors.FirstOrDefault(x => x.Code == item.code) == null)
				{
					Major newMajor = new Major();
					newMajor.Code = item.code;
					newMajor.Name = item.name;
					db.Majors.Add(newMajor);
				}
			}
			db.SaveChanges();
		}

		public void SynCourse()
		{
			var course = api.getCourses();
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