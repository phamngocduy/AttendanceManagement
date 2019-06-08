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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

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
			var allUser = db.Users.ToList();
			return View(allUser);
		}
		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> Create(Group group, List<int> MemberUserID)
		{
			Group nGroup = new Group();
			nGroup.GroupName = group.GroupName;
			nGroup.GroupDescription = group.GroupDescription;
			nGroup.GroupType = false;
			nGroup.GroupParent = 1;
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
			if (MemberUserID != null)
			{
				foreach (var item in MemberUserID)
				{
					var newMember = db.Users.FirstOrDefault(x => x.ID == item);
					if (!nGroup.Users.Contains(newMember))
					{
						nGroup.Users.Add(newMember);
					}
				}

			}

			db.SaveChanges();
			string nGroupID = db.Groups.OrderByDescending(x => x.ID).First().ID.ToString();
			Session["GroupID"] = nGroupID;
			return RedirectToAction("Detail", new { id = nGroupID });
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
			var memberlist = group.Users.ToList().OrderBy(x => x.FirstName);
			List<Group> tempList = GroupParentwithGroup(group);
			ViewBag.GroupParent = tempList;
			ViewBag.Owner = group.Users1.ToList();

			var listOwner = group.Users1.ToList();
		
			var listGroupOwner = group.Group11.ToList();
			ViewBag.GroupManager = listGroupOwner;
			foreach (var item in listGroupOwner)
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
			int lengthImport = 0;
			int lengthExits = 0;
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
								lengthImport++;
							}
							else
							{
								user.AvatarLink = s.AvatarLink;
								user.AvatarBase64 = s.AvatarBase64;
								user.DoB = s.DoB;
								user.Email = s.Email;
								user.FirstName = s.FirstName;
								user.LastName = s.LastName;
								user.Note = s.Note;
								user.PhoneNumber = s.PhoneNumber;
								user.PlaceofBirth = s.PlaceofBirth;
								user.StID = s.StID;
								user.UserID = aspUser.Id;
								user.Gender = s.Gender;
								db.SaveChanges();
								if ((group.Users.FirstOrDefault(x => x.StID == s.StID)) == null)
								{
									group.Users.Add(user);
									AddParentGroup(group, user);
									lengthImport++;
								}
								else
								{
									lengthExits++;
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

			HttpCookie cookieImportstudent = new HttpCookie("jusy_Importstudent", lengthImport + "");
			HttpCookie cookieExitsStudent = new HttpCookie("just_ExitsStudent", lengthExits + "");
			HttpContext.Response.Cookies.Add(cookieImportstudent);
			HttpContext.Response.Cookies.Add(cookieExitsStudent);

			TempData.Remove("Excelstudent");
			return RedirectToAction("Detail", new { id = Session["GroupID"] });
		}
		//Users : Sinh vien
		//User1 : groupowner

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

		private  List<User> ConvertDataTable<T>(DataTable dt)
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
				us.DoB = DateTime.Parse(row[5].ToString());
				us.PlaceofBirth = row[6].ToString();
				us.Email = row[7].ToString();
				us.Note = row[8].ToString();
				var image = GenerateDefaultAvatar(row[3].ToString(), row[2].ToString());
				//save link
				var avatarPath = "/LoginManagement/Avatars";
				var newFileName = Path.Combine(avatarPath, row[1].ToString()+".png");
				var newFileLocation = HttpContext.Server.MapPath(newFileName);
				if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
				}
				Image img = Image.FromStream(image);
				img.Save(newFileLocation);
				string filename = newFileName.Replace("~", string.Empty);
				us.AvatarLink = filename;
				//resize 32*32 and save
				Image avatar = resizeImage(img, 32, 32);
				var Avatarbase64 = ImageToBase64(avatar);
				us.AvatarBase64 = Avatarbase64;

				data.Add(us);
			}
			return data;
		}
		public Image resizeImage(Image img, int width, int height)
		{
			Bitmap b = new Bitmap(width, height);
			Graphics g = Graphics.FromImage((Image)b);

			g.DrawImage(img, 0, 0, width, height);
			g.Dispose();

			return (Image)b;
		}
		public MemoryStream GenerateDefaultAvatar(string firstName, string lastName)
		{
			var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();
			var random = new Random();
			var bgColour = String.Format("#{0:X6}", random.Next(0x1000000));

			var bmp = new Bitmap(256, 256);
			var sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			var font = new Font("Arial", 120, FontStyle.Bold, GraphicsUnit.Pixel);
			var graphics = Graphics.FromImage(bmp);

			graphics.Clear(Color.Transparent);
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			using (Brush b = new SolidBrush(ColorTranslator.FromHtml(bgColour)))
			{
				graphics.FillEllipse(b, new Rectangle(0, 0, 256, 256));
			}
			graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 128, 138, sf);
			graphics.Flush();

			var ms = new MemoryStream();
			bmp.Save(ms, ImageFormat.Png);
			return ms;
		}
		public string ImageToBase64(Image img)
		{
				using (MemoryStream ms = new MemoryStream())
				{
					string base64String;
					img.Save(ms, ImageFormat.Png);
					byte[] imageBytes = ms.ToArray();
					base64String = Convert.ToBase64String(imageBytes);
					return base64String;
				}
			
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
		public ActionResult AddStudent()
		{
			var user = db.Users.ToList();
			return View(user);
		}
		[HttpPost]
		public ActionResult AddStudent(string email)
		{
			var user = db.Users.FirstOrDefault(x => x.Email == email);
			int groupID = int.Parse(Session["GroupID"].ToString());
			var group = db.Groups.FirstOrDefault(x => x.ID == groupID);
			if (!group.Users.Contains(user))
			{
				group.Users.Add(user);
				HttpCookie cookieAddStudent = new HttpCookie("just_addStudent", "success");
				HttpContext.Response.Cookies.Add(cookieAddStudent);
			}
			else
			{
				HttpCookie cookieAddStudent = new HttpCookie("just_addStudent", "fail");
				HttpContext.Response.Cookies.Add(cookieAddStudent);
			}
			db.SaveChanges();
			return RedirectToAction("Detail", new { id = groupID });
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
			if (!group.Users1.Contains(user))
			{
				group.Users1.Add(user);
				HttpCookie cookieaddOwner = new HttpCookie("just_addOwner", "success");
				HttpContext.Response.Cookies.Add(cookieaddOwner);
			}
			else
			{
				HttpCookie cookieaddOwner = new HttpCookie("just_addOwner", "fail");
				HttpContext.Response.Cookies.Add(cookieaddOwner);
			}
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
			
			if (!group.Group11.Contains(addGroup))
			{
				group.Group11.Add(addGroup);
				HttpCookie cookieaddGroup = new HttpCookie("just_addGroup", "success");
				HttpContext.Response.Cookies.Add(cookieaddGroup);
			}
			else
			{
				HttpCookie cookieaddGroup = new HttpCookie("just_addGroup", "fail");
				HttpContext.Response.Cookies.Add(cookieaddGroup);
			}

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
