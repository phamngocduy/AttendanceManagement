using ExcelDataReader;
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
	[Authorize]
	public class GroupController : Controller
	{
		cap21t4Entities db = new cap21t4Entities();
		// GET: Group
		public ActionResult Index()
		{
			TempData.Remove("Excelstudent");
			List<Group> tempList = db.Groups.ToList();
			var grouplist = tempList.Where(g => g.GroupParent == null).ToList();
			ViewBag.User = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
			return View(grouplist);
		}
		[HttpGet]
		public ActionResult Create()
		{
			List<Group> ParentList = db.Groups.OrderByDescending(x => x.ID).ToList();
			ParentList.RemoveRange(ParentList.Count - 2, 2);
			ViewBag.Parent = ParentList;
			return View(ViewBag.Parent);
		}
		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> CreateAsync(Group group)
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

				User nUser = new User();
				var user = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
				if (user == null)
				{
					var aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == User.Identity.Name);
					if (aspUser == null)
					{
						var newuser = new ApplicationUser { UserName = User.Identity.Name, Email = User.Identity.Name };
						var result = await UserManager.CreateAsync(newuser);
						aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == User.Identity.Name);
					}
					nUser.UserID = aspUser.Id;
					db.Users.Add(nUser);
					db.SaveChanges();
				}
				else
				{
					nUser = user;
				}
				//add owner
				nGroup.Users1.Add(nUser);
				//add member
				nGroup.Users.Add(nUser);

				db.SaveChanges();
				TempData["SuccessMessage"] = "Created a new group successfully!";
				string nGroupID = db.Groups.OrderByDescending(x => x.ID).First().ID.ToString();
				Session["GroupID"] = nGroupID;
				return RedirectToAction("Detail", new { id = nGroupID });
			}
			TempData["ErrorMessage"] = "Group name cannot be empty!";
			return RedirectToAction("Create");

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
		public async System.Threading.Tasks.Task<ActionResult> CreateGroup(Group group)
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
			
				User nUser = new User();
				var user = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
				if (user == null)
				{
					var aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == User.Identity.Name);
					if (aspUser == null)
					{
						var newuser = new ApplicationUser { UserName = User.Identity.Name, Email = User.Identity.Name };
						var result = await UserManager.CreateAsync(newuser);
						aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == User.Identity.Name);
					}
					nUser.UserID = aspUser.Id;
					db.Users.Add(nUser);
					db.SaveChanges();
				}
				else
				{
					nUser = user;
				}
				//add owner
				nGroup.Users1.Add(nUser);
				//add member
				nGroup.Users.Add(nUser);

				db.SaveChanges();
				string nGroupID = db.Groups.OrderByDescending(x => x.ID).First().ID.ToString();
				Session["GroupID"] = nGroupID;
				return RedirectToAction("Detail", new { id = nGroupID });
			}
			return View(group);
		}
		public ActionResult Detail(string id)
		{
			TempData.Remove("Excelstudent");
			int groupID = int.Parse(id);
			Session["GroupID"] = id;
			var group = db.Groups.FirstOrDefault(x => x.ID == groupID);
			var memberlist = group.Users.ToList().OrderBy(x => x.LastName);
			List<Group> tempList = GroupParentwithGroup(group);
			ViewBag.GroupParent = tempList;
			var listOwner = group.Users1.ToList();
			ViewBag.Owner = listOwner;
			var listGroupOwner = group.Group11.ToList();
			ViewBag.GroupManager = listGroupOwner;
			foreach( var item in listGroupOwner)
			{
				listOwner.AddRange(item.Users);
			}
			var user = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
			if (listOwner.Contains(user))
			{
				ViewBag.UserRole = "Group Owner";
			}
			else
			{
				ViewBag.UserRole = "Member";
			}

			return View(memberlist);
		}


		public ActionResult Import(string id)
		{
			TempData.Keep();
			return View();
		}

		[HttpPost]
		public ActionResult ReadExcel()
		{
			List<User> lstStudent = new List<User>();
			if (ModelState.IsValid)
			{
				try
				{
					string filePath = string.Empty;
					if (Request != null)
					{
						HttpPostedFileBase file = Request.Files["fileupload"];
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
							lstStudent = ConvertDataTable<User>(dt);
							TempData["Excelstudent"] = lstStudent.OrderBy(x => x.LastName).ToList();

						}
					}
				}
				catch (Exception ex)
				{
					return new JsonResult { Data = ex.Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
				}

			}
			return RedirectToAction("Import");
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
							var aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == s.Email);
							if (aspUser == null)
							{
								var newuser = new ApplicationUser { UserName = s.Email, Email = s.Email };
								var result = await UserManager.CreateAsync(newuser);
								aspUser = db.AspNetUsers.FirstOrDefault(x => x.Email == s.Email);
							}
							var group = db.Groups.FirstOrDefault(x => x.ID == groupID);

							var user = db.Users.FirstOrDefault(x => x.UserID == aspUser.Id);
							if (user == null)
							{
								s.UserID = aspUser.Id;
								db.Users.Add(s);
								db.SaveChanges();
								group.Users.Add(s);
								AddParentGroup(group, s);
								db.SaveChanges();
								length++;
							}
							else
							{
								user = s;
								user.UserID = aspUser.Id;
								db.SaveChanges();
								if ((group.Users.FirstOrDefault(x => x.StID == s.StID)) == null)
								{
									group.Users.Add(user);
									AddParentGroup(group, user);
									db.SaveChanges();
									length++;
								}
							}
						}
						db.SaveChanges();
					}
				
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			TempData.Remove("Excelstudent");
			return RedirectToAction("Detail", new { id = Session["GroupID"] });
		}
		//Users : Sinh vien
		//User1 : groupowner

		public void AddUser()
		{

		}

		public void AddParentGroup(Group g, User u)
		{
			if (g.Group2 != null && g.Group2.ID != 2 && g.Group2.ID != 1)
			{
				if (g.Group2.Users.Contains(u) == false)
				{
					g.Group2.Users.Add(u);
				}
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
				us.LastName = row[2].ToString();
				us.FirstName = row[3].ToString();
				if (row[4].ToString() == "0")
				{
					us.Gender = false;
				}
				else
				{
					us.Gender = true;
				}
				us.DoB = DateTime.ParseExact(row[5].ToString(), "dd/mm/yyyy", null);
				us.PlaceofBirth = row[6].ToString();
				us.Email = row[7].ToString();
				us.Note = row[8].ToString();
				data.Add(us);
			}
			return data;
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
				return RedirectToAction("Index");
			}
			else
			{
				return RedirectToAction("Edit");
			}
		}

		[HttpGet]
		public ActionResult AddGroupOwner()
		{
			var user = db.Users.ToList();
			return View(user);
		}
		[HttpPost]
		public ActionResult AddGroupOwner(string email)
		{
			var user = db.Users.FirstOrDefault(x => x.Email == email);
			int groupID = int.Parse(Session["GroupID"].ToString());
			var group = db.Groups.FirstOrDefault(x => x.ID == groupID);
			group.Users1.Add(user);
			db.SaveChanges();
			return RedirectToAction("Detail", new { id = groupID });
		}
		[HttpGet]
		public ActionResult AddGroupManager()
		{
			var group = db.Groups.Where(x => x.GroupParent != null);
			return View(group);
		}
		[HttpPost]
		public ActionResult AddGroupManager(string groupID)
		{
			int addgroupID = int.Parse(groupID);
			var addGroup = db.Groups.FirstOrDefault(x => x.ID == addgroupID);
			int editgroupID = int.Parse(Session["GroupID"].ToString());
			var group = db.Groups.FirstOrDefault(x => x.ID == editgroupID);
			group.Group11.Add(addGroup);
			db.SaveChanges();
			return RedirectToAction("Detail", new { id = editgroupID });
		}

		public ActionResult Delete(string id)
		{
			int groupID = int.Parse(id);
			Session["DeleteGroupID"] = groupID;
			var group = db.Groups.FirstOrDefault(x => x.ID == groupID);
			List<Group> tempList = GroupParent(group);
			return View(tempList);

		}
		[HttpPost]
		public ActionResult Delete(IEnumerable<int> ids)
		{
			int deletegroup = int.Parse(Session["DeleteGroupID"].ToString());
			var group = db.Groups.FirstOrDefault(x => x.ID == deletegroup);
			foreach (int item in ids)
			{
				var groupParent = db.Groups.FirstOrDefault(x => x.ID == item);
				foreach (var member in group.Users)
				{
					groupParent.Users.Remove(member);
				}
			}
			foreach (var gr in group.Group1)
			{
				db.Groups.Remove(gr);
			}
			group.Users.Clear();
			group.Users1.Clear();
			group.Group1.Clear();
			group.Group11.Clear();
			db.Groups.Remove(group);
			db.SaveChanges();

			return RedirectToAction("Index");
		}
		public List<Group> GroupParent(Group g)
		{
			List<Group> parentGroup = new List<Group>();
			GetParentGroup(g, parentGroup);
			return parentGroup;

		}
		public List<Group> GroupParentwithGroup(Group g)
		{
			List<Group> parentGroup = new List<Group>();
			parentGroup.Add(g);
			GetParentGroup(g, parentGroup);
			return parentGroup;

		}

		public void GetParentGroup(Group g, List<Group> list)
		{
			if (g.Group2 != null && g.Group2.ID != 2 && g.Group2.ID != 1)
			{
				list.Add(g.Group2);
				GetParentGroup(g.Group2, list);
			}
		}

		public ActionResult DeletePrivateGroup(int id)
		{
			var group = db.Groups.FirstOrDefault(x => x.ID == id);
			group.Users.Clear();
			group.Users1.Clear();
			group.Group1.Clear();
			group.Group11.Clear();
			db.Groups.Remove(group);
			db.SaveChanges();
			return Json(true, JsonRequestBehavior.DenyGet);
		}

	}
}
