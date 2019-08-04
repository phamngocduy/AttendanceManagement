using AttendanceManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceManagement.Controllers
{
	public class FacultyController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
		APIController api = new APIController();
		// GET: Faculty


		public ActionResult Index()
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
			string groupdata = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getAllGroups");
			List<GroupModel> group = JsonConvert.DeserializeObject<List<GroupModel>>(groupdata);
			ViewBag.GroupMap = group.ToList();
			ViewBag.MajorList = db.Majors.ToList();

			return View();
		}
		[HttpPost]
		public ActionResult CreateFaculty1(Faculty faculty, List<int> MajorID)
		{
			Faculty newfaculty = new Faculty();
			newfaculty.Name = faculty.Name;
			newfaculty.Description = faculty.Description;
			newfaculty.GroupID = faculty.GroupID;
			if (MajorID != null)
			{
				foreach (var item in MajorID)
				{
					var major = db.Majors.FirstOrDefault(x => x.ID == item);
					newfaculty.Majors.Add(major);
				}

			}

			db.Faculties.Add(newfaculty);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		public ActionResult MajorList()
		{
			var major = db.Majors.ToList();
			return View(major);
		}
		[HttpPost]
		public int SynMajor()
		{
			int newMajorCount = 0;
			APIController api = new APIController();
			string data = api.ReadData("https://sodaubai.vanlanguni.edu.vn/API/getMajors");
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
					newMajorCount++;
				}
			}
			return newMajorCount;
		}

	}
}