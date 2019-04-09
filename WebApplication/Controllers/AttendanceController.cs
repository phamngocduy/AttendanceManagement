using System;
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
		cap21t4Entities db = new cap21t4Entities();
		// GET: Attendance
		public ActionResult Index()
		{
			var classlist = db.Classes.ToList();
			return View(classlist);
		}

		public ActionResult CreateClassView()
		{
			return PartialView("CreateClassView");
		}

		[HttpPost]
		public ActionResult CreateClass(Class Class)
		{
			if (ModelState.IsValid)
			{
				Class nClass = new Class();
				nClass.ClassName = Class.ClassName;
				nClass.StartDate = Class.StartDate;
				nClass.CreatedDate = DateTime.Now.Date;
				nClass.CreatedBy = User.Identity.Name;
				db.Classes.Add(nClass);
				db.SaveChanges();
				return RedirectToAction("manageClass");
			}
			return RedirectToAction("Index");

		}

		public ActionResult Edit(string id)
		{
			var ClassID = int.Parse(id);
			var editClass = db.Classes.FirstOrDefault(x => x.ID == ClassID);
			return PartialView("EditClassView", editClass);
		}

		[HttpPost]
		public ActionResult Edit(Class editClass)
		{
			var eClass = db.Classes.FirstOrDefault(x => x.ID == editClass.ID);

			if (ModelState.IsValid)
			{
				eClass.StartDate = editClass.StartDate;
				eClass.Description = editClass.Description;
				eClass.ModifiedBy = User.Identity.Name;
				eClass.ModifiedDate = DateTime.Now.Date;
				db.SaveChanges();
			}
			return RedirectToAction("Index");

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

		public ActionResult MajorList()
		{
			SynMajor();
			var major = db.Majors.ToList();
			return View(major);
		}
		public void SynMajor()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getMajors");

			List<MajorModel> major = JsonConvert.DeserializeObject<List<MajorModel>>(data);
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
    }
}