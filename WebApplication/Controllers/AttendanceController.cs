using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using System.Web.Mvc;
using System.IO;
using System.Data;
using ExcelDataReader;

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

        public ActionResult Index1()
        {
            return View();
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

        public ActionResult EditClass1()
        {
            return View();
        }
        public ActionResult AttendanceExcel()
        {
            return View();
        }

        public ActionResult Import(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadExcel()
        {
            
            if (ModelState.IsValid)
            {

                string filePath = string.Empty;
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["file"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {

                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        filePath = path + Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        file.SaveAs(filePath);
                        Stream stream = file.InputStream;
                        // We return the interface, so that
                        IExcelDataReader reader = null;
                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return RedirectToAction("Index");
                        }
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        //reader.IsFirstRowAsColumnNames = true;
                        //DataSet result = reader.AsDataSet();
                        reader.Close();
                        //delete the file from physical path after reading 
                        string filedetails = path + fileName;
                        FileInfo fileinfo = new FileInfo(filedetails);
                        if (fileinfo.Exists)
                        {
                            fileinfo.Delete();
                        }
                        DataTable dt = result.Tables[0];
                        
                        //TempData["Excelstudent"] = ;
                    }
                }

            }
            // var files = Request.Files;
            return View();
            //return new JsonResult { Data = , JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}