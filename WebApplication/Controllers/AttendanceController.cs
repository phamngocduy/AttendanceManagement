﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WebApplication.Controllers
{
	[Authorize]
	public class AttendanceController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
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
		public ActionResult manageClass()
		{
			var user = db.CourseMembers.ToList();
			ViewData["students"] = user;
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
		public ActionResult DetailClass1()
		{
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
		public ActionResult AddStudent()
		{
			return View();
		}
		public ActionResult SynCourse()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getCourses");
			CourseModel course = JsonConvert.DeserializeObject<CourseModel>(data);
			foreach (var item in course.Courses)
			{
				var SynCourse = db.Courses.FirstOrDefault(x => x.Code == item.Code && x.Type1 == item.Type1 && x.Type2 == item.Type2);
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