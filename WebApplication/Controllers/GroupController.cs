using Excel;
using WebApplication.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace WebApplication.Controllers
{
	public class GroupController : Controller
	{
		cap21t4Entities db = new cap21t4Entities();
		// GET: Group
		public ActionResult Index()
		{
			List<Group> tempList;
			tempList = db.Groups.ToList();
			var grouplist = tempList.Where(g => g.GroupParent == null).ToList();
			return View(grouplist);
		}
		[Authorize]
		[HttpGet]
		public ActionResult Create()
		{
			Group model = new Group();
			List<Group> ParentList = db.Groups.OrderByDescending(x => x.ID).ToList();
			ParentList.RemoveRange(ParentList.Count - 2, 2);
			ViewBag.Parent = ParentList;
			return View(model);
		}
		[HttpPost]
		public ActionResult Create(Group group)
		{
			if (ModelState.IsValid)
			{
				Group nGroup = new Group();
				nGroup.GroupName = group.GroupName;
				nGroup.GroupDescription = group.GroupDescription;
				nGroup.GroupType = group.GroupType;
				nGroup.GroupParent = group.GroupParent;
				nGroup.CreatedDate = DateTime.Now.Date;
				nGroup.CreatedBy = User.Identity.Name;
				db.Groups.Add(nGroup);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(group);
		}
		[Authorize]
		// GET: Groups/Create
		[HttpGet]
		public ActionResult CreateGroup(string id)
		{
			Group model = new Group();
			String parentgroupName;
			var GroupParentID = int.Parse(id);
			if (GroupParentID == 1 || GroupParentID == 2)
			{
				parentgroupName = "No Parent Group";
			}
			else
			{
				parentgroupName = db.Groups.First(x => x.ID == GroupParentID).GroupName;
			}

			ViewBag.ParentID = GroupParentID;
			ViewBag.ParentName = parentgroupName;
			ViewBag.GroupType = db.Groups.First(x => x.ID == GroupParentID).GroupType;
			return View(model);
		}

		// POST: Groups/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateGroup(Group group)
		{
			if (ModelState.IsValid)
			{
				Group nGroup = new Group();
				nGroup.GroupName = group.GroupName;
				nGroup.GroupDescription = group.GroupDescription;
				nGroup.GroupType = group.GroupType;
				nGroup.GroupParent = group.GroupParent;
				nGroup.CreatedDate = DateTime.Now.Date;
				nGroup.CreatedBy = User.Identity.Name;
				db.Groups.Add(nGroup);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(group);
		}
		public ActionResult Detail(string id)
		{
			int groupID = int.Parse(id);
			Session["GroupID"] = id;
			var memberlist = (from us in db.Users
							  from gr in us.Groups.Where(x => x.ID == groupID)
							  select us);
			List<User> users = memberlist.ToList();
			return View(memberlist);
		}

		public async System.Threading.Tasks.Task InsertUserAsync(string sEmail)
		{
			var info = await AuthenticationManager.GetExternalLoginInfoAsync();

			var user = new ApplicationUser { UserName = sEmail, Email = sEmail };
			var result = await UserManager.CreateAsync(user);
			if (result.Succeeded)
			{
				result = await UserManager.AddLoginAsync(user.Id, info.Login);
				//if (result.Succeeded)
				//{
				//	await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				//	return RedirectToLocal(returnUrl);
				//}
			}
		}
		public ActionResult Import(string id)
		{
			return View();
		}

		[HttpPost]
		public ActionResult ReadExcel()
		{
			List<User> lstStudent = new List<User>();
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
						reader.IsFirstRowAsColumnNames = true;
						DataSet result = reader.AsDataSet();
						reader.Close();
						//delete the file from physical path after reading 
						string filedetails = path + fileName;
						FileInfo fileinfo = new FileInfo(filedetails);
						if (fileinfo.Exists)
						{
							fileinfo.Delete();
						}
						DataTable dt = result.Tables[0];
						lstStudent = ConvertDataTable<User>(dt);
						TempData["Excelstudent"] = lstStudent;
					}
				}

			}
			// var files = Request.Files;

			return new JsonResult { Data = lstStudent, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		public async System.Threading.Tasks.Task<ActionResult> InsertExcelData()
		{
			int groupID = int.Parse(Session["GroupID"].ToString());
			int length = 0;
			try
			{
				if (TempData["Excelstudent"] != null)
				{
					List<User> lstStudent = (List<User>)TempData["Excelstudent"];
					using (cap21t4Entities db = new cap21t4Entities())
					{
						foreach (var s in lstStudent)
						{
							var userid = db.AspNetUsers.FirstOrDefault(x => x.Email == s.Email);
							if (userid == null)
							{
								await InsertUserAsync(s.Email);
							}
							var uID = db.AspNetUsers.FirstOrDefault(x => x.Email == s.Email).Id;

							if (db.Users.FirstOrDefault(x => x.StID == s.StID) == null)
							{
								s.UserID = uID;
								db.Users.Add(s);
								db.SaveChanges();
							}
							Group group = db.Groups.FirstOrDefault(x => x.ID == groupID);	
							if(group.Users.Where(x=>x.StID==s.StID)==null)
							{
								group.Users.Add(s);
								AddParentGroup(group, s);
								db.SaveChanges();
								length++;
							}
							
						}
						db.SaveChanges();
						
					}
					//return RedirectToAction("Detail", new { id = groupID });
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			return new JsonResult { Data = length, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		public void AddParentGroup(Group g, User u)
		{
			if (g.Group2 != null && g.Group2.ID != 2)
			{
				g.Group2.Users.Add(u);
				AddParentGroup(g.Group2, u);
			}
		}

		private static List<User> ConvertDataTable<T>(DataTable dt)
		{
			List<User> data = new List<User>();
			dt.Rows.RemoveAt(0);
			dt.Rows.RemoveAt(0);
			dt.Rows.RemoveAt(0);
			dt.Rows.RemoveAt(0);
			dt.Rows.RemoveAt(0);
			dt.Rows.RemoveAt(0);

			DataRowCollection list = dt.Rows;
			foreach (DataRow row in list)
			{

				if (String.IsNullOrEmpty(row[0].ToString()))
				{
					break;
				}
				User us = new User();

				us.StID = row[1].ToString();
				us.FullName = row[2].ToString()+ row[3].ToString();
				//us.PhoneNumber = row[3].ToString();
				string s = row[4].ToString();
				us.DoB = DateTime.ParseExact(row[4].ToString(),"d/M/yyyy", null);
				us.Email = row[5].ToString();
				us.Note = row[6].ToString();
				data.Add(us);
			}
			return data;
		}
		private static T GetItem<T>(DataRow dr)
		{
			Type temp = typeof(T);
			T obj = Activator.CreateInstance<T>();


			foreach (DataColumn column in dr.Table.Columns)
			{

				foreach (PropertyInfo pro in temp.GetProperties())
				{
					string s = dr[column.ColumnName].ToString();
					if (pro.Name == column.ColumnName && dr.Table.Rows != null)
					{

						pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
					}

					else
						continue;
				}
			}
			return obj;
		}

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}
		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}
		public ActionResult ImportStudent()
		{
			return View();
		}
		[HttpGet]
		public ActionResult Edit(string id)
		{
			int groupID = int.Parse(id);
			Group editGroup = db.Groups.FirstOrDefault(x => x.ID == groupID);

			ViewBag.Parent = editGroup.Group1;
			return View(editGroup);
		}
		[HttpPost]
		public ActionResult Edit(Group editgroup)
		{

			Group group = db.Groups.FirstOrDefault(x => x.ID == editgroup.ID);
			if (ModelState.IsValid)
			{
				group.GroupName = editgroup.GroupName;
				group.GroupDescription = editgroup.GroupDescription;
				group.ModifiedBy = User.Identity.Name;
				group.ModifiedDate = DateTime.Now.Date;
				db.SaveChanges();
			}
			return RedirectToAction("Index");
		}
	}
}
