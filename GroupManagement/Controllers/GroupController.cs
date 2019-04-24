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
			List<Group> tempList = db.Groups.ToList();
			var grouplist = tempList.Where(g => g.GroupParent == null).ToList();
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
				//add owner
				var owner = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
				nGroup.Users1.Add(owner);

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
				//add owner
				var owner = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
				nGroup.Users1.Add(owner);

				db.SaveChanges();
				string nGroupID = db.Groups.OrderByDescending(x => x.ID).First().ID.ToString();
				Session["GroupID"] = nGroupID;
				return RedirectToAction("Detail", new { id = nGroupID });
			}
			return View(group);
		}
        public ActionResult Detail(string id)
        {
			int groupID = int.Parse(id);
			Session["GroupID"] = id;
			var group = db.Groups.FirstOrDefault(x => x.ID == groupID);
			var memberlist = group.Users.ToList();
			ViewBag.GroupName = group.GroupName;
			ViewBag.Owner = group.Users1.ToList();
			ViewBag.GroupManager = group.Group11.ToList();
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
							TempData["Excelstudent"] = lstStudent;

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
							var user = db.Users.FirstOrDefault(x => x.StID == s.StID);
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
                us.FullName = row[2].ToString() + " " + row[3].ToString();
                //us.PhoneNumber = row[3].ToString();
                string s = row[4].ToString();
                us.DoB = DateTime.ParseExact(row[4].ToString(), "dd/mm/yyyy", null);
                us.Email = row[5].ToString();
                us.Note = row[6].ToString();
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
                TempData["SuccessMessage"] = "Edited a group successfully!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Group name cannot be empty!";
            return RedirectToAction("Edit");
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
			var user = db.Users.FirstOrDefault(x=>x.Email==email);
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
	}
}
