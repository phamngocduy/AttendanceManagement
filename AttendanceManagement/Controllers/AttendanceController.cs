using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using System.Text;
using System.Transactions;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using ZXing;
using ZXing.QrCode;
using System.Web.Services;
using System.Data;
using ClosedXML.Excel;
using System.Globalization;

namespace AttendanceManagement.Controllers
{
	[Authorize]
	public class AttendanceController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
		APIController api = new APIController();
		private static List<string> AttendanceList;
		static AttendanceController()
		{
			AttendanceList = new List<string>();
		}
		// GET: Attendance
		public ActionResult Index()
		{
			var courselist = db.Courses.ToList();
			return View(courselist);

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
		

		public ActionResult studentDetailClass(string id)
		{
			Session["CourseID"] = id;
			int courseID = int.Parse(id);
			ViewBag.Course = db.Courses.FirstOrDefault(x => x.ID == courseID).CourseName;
			ViewBag.tab = "tab1";
			if (Session["tabactive"] != null)
			{
				ViewBag.tab = Session["tabactive"].ToString();
				Session.Remove("tabactive");
			}

			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.FirstName).ToList();
			ViewData["students"] = student;
			ViewBag.StudentCount = student.Count;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.FirstName = iten.FirstName;
				attendanceview.LastName = iten.LastName;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				foreach (var item in session)
				{
					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime)item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance != null)
					{
						detail.Status = attendance.Status;

						if (attendance.Status != "0")
						{
							TotalPresent++;
						}
						if (attendance.Status != "0")
						{
							TotalPoint = TotalPoint + int.Parse(attendance.Status);
						}

						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}
			list = list.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
			ViewData["attendance"] = list;
			var topAttendance = list.OrderByDescending(x => x.TotalPresent).Take(10).ToList();
			ViewBag.topAttendance = topAttendance;
			var lastAttendance = list.OrderBy(x => x.TotalPresent).Take(10).ToList();
			ViewBag.lastAttendance = lastAttendance;

			return View();
		}

		public ActionResult lecturerDetailClass(string id)
		{
			Session["CourseID"] = id;
			int courseID = int.Parse(id);
			ViewBag.Course = db.Courses.FirstOrDefault(x => x.ID == courseID).CourseName;
			ViewBag.tab = "tab1";
			if (Session["tabactive"] != null)
			{
				ViewBag.tab = Session["tabactive"].ToString();
				Session.Remove("tabactive");
			}

			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.FirstName).ToList();
			ViewData["students"] = student;
			ViewBag.StudentCount = student.Count;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.FirstName = iten.FirstName;
				attendanceview.LastName = iten.LastName;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				foreach (var item in session)
				{
					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime)item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance != null)
					{
						detail.Status = attendance.Status;

						if (attendance.Status != "0")
						{
							TotalPresent++;
						}
						if (attendance.Status != "0")
						{
							TotalPoint = TotalPoint + int.Parse(attendance.Status);
						}

						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}
			list = list.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
			ViewData["attendance"] = list;
			var topAttendance = list.OrderByDescending(x => x.TotalPresent).Take(10).ToList();
			ViewBag.topAttendance = topAttendance;
			var lastAttendance = list.OrderBy(x => x.TotalPresent).Take(10).ToList();
			ViewBag.lastAttendance = lastAttendance;

			return View();
		}

		public ActionResult staffDetailClass(string id)
		{
			Session["CourseID"] = id;
			int courseID = int.Parse(id);
			ViewBag.Course = db.Courses.FirstOrDefault(x => x.ID == courseID).CourseName;
			ViewBag.tab = "tab1";
			if (Session["tabactive"] != null)
			{
				ViewBag.tab = Session["tabactive"].ToString();
				Session.Remove("tabactive");
			}

			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.FirstName).ToList();
			ViewData["students"] = student;
			ViewBag.StudentCount = student.Count;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.FirstName = iten.FirstName;
				attendanceview.LastName = iten.LastName;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				foreach (var item in session)
				{
					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime)item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance != null)
					{
						detail.Status = attendance.Status;

						if (attendance.Status != "0")
						{
							TotalPresent++;
						}
						if (attendance.Status != "0")
						{
							TotalPoint = TotalPoint + int.Parse(attendance.Status);
						}

						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}
			list = list.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
			ViewData["attendance"] = list;
			var topAttendance = list.OrderByDescending(x => x.TotalPresent).Take(10).ToList();
			ViewBag.topAttendance = topAttendance;
			var lastAttendance = list.OrderBy(x => x.TotalPresent).Take(10).ToList();
			ViewBag.lastAttendance = lastAttendance;

			return View();
		}
		public ActionResult AddStudent(string groupname)
		{
			string groupdata = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getAllGroups");
			List<GroupModel> group = JsonConvert.DeserializeObject<List<GroupModel>>(groupdata);
			ViewBag.Group = group;
			if (groupname != null)
			{
				string userdata = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getMember?groupname=" + groupname);
				List<UserModel> user = JsonConvert.DeserializeObject<List<UserModel>>(userdata);
				return View(user);
			}

			return View();

		}
		[HttpPost]
		public ActionResult AddStudent(List<string> studentlist)
		{
			int iCountAddStudent = 0;
			int iCountStudentInCourse = 0;
			foreach (var item in studentlist)
			{
				string studentdata = api.ReadData("https://fitlogin.vanlanguni.edu.vn/GroupManagement/api/getUserInfo?searchString=" + item);
				UserModel student = JsonConvert.DeserializeObject<UserModel>(studentdata);
				var courseID = int.Parse(Session["CourseID"].ToString());
				var course = db.Courses.FirstOrDefault(x => x.ID == courseID);
				var StudentInCourse = course.CourseMembers.FirstOrDefault(x => x.StudentID == student.StID);
				if (StudentInCourse == null)
				{
					CourseMember newMember = new CourseMember();
					newMember.CourseID = courseID;
					newMember.StudentID = student.StID;
					newMember.FirstName = student.FirstName;
					newMember.LastName = student.LastName;
					newMember.Email = student.Email;
					newMember.DoB = student.DoB;
					newMember.Avatar = student.Avatar;
					course.CourseMembers.Add(newMember);
					iCountAddStudent++;
				}
				else
				{
					iCountStudentInCourse++;
				}

			}
			db.SaveChanges();

			HttpCookie cookieCountStudentAdd = new HttpCookie("jusy_CountStudentAdd", iCountAddStudent + "");
			HttpCookie cookieCountStudentInCourse = new HttpCookie("just_CountStudentInCourse", iCountStudentInCourse + "");
			HttpContext.Response.Cookies.Add(cookieCountStudentAdd);
			HttpContext.Response.Cookies.Add(cookieCountStudentInCourse);

			Session["tabactive"] = "tab2";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		public ActionResult GetDateAttendance(string id)
		{
			Session["SessionID"] = id;
			int sessionid = int.Parse(id);
			DateTime? date = db.Sessions.FirstOrDefault(x => x.ID == sessionid).Date;
			string json = JsonConvert.SerializeObject(date.Value.ToShortDateString());

			return Json(json, JsonRequestBehavior.DenyGet);
		}

		[HttpPost]
		public JsonResult CheckAttendance(List<AttendanceModel> attendance)
		{
			using (var scope = new TransactionScope())
			{
				int sessionID = int.Parse(Session["SessionID"].ToString());
				db.Attendances.RemoveRange(db.Attendances.Where(x => x.SessionID == sessionID));
				foreach (var item in attendance)
				{
					Attendance att = new Attendance();
					att.MemberID = item.memberID;
					att.SessionID = sessionID;
					att.Status = item.status;
					att.Note = item.note;
					db.Attendances.Add(att);
				}
				db.SaveChanges();
				scope.Complete();
				Session.Remove("SessionID");
				Session["tabactive"] = "tab1";
			}

			return Json("true");
		}

		[HttpPost]
		public JsonResult EditAttendance(List<AttendanceModel> attendance)
		{
			using (var scope = new TransactionScope())
			{
				int sessionID = int.Parse(Session["SessionID"].ToString());
				foreach (var item in attendance)
				{
					var editAttendance = db.Attendances.FirstOrDefault(x => x.MemberID == item.memberID && x.SessionID == sessionID);
					editAttendance.Status = item.status;
					editAttendance.Note = item.note;
					db.SaveChanges();
				}
				scope.Complete();
				Session.Remove("SessionID");
				Session["tabactive"] = "tab1";
			}

			return Json("true");
		}

		public ActionResult Generate(string id)
		{
			AttendanceList.Add(id);
			QRCodeModel qrcode = new QRCodeModel();
			qrcode.QRCodeText = id;
			try
			{
				qrcode.QRCodeImagePath = GenerateQRCode(id);
				ViewBag.Message = "QR Code Generated successfully";
				int CourseID = int.Parse(Session["CourseID"].ToString());
				int SessionID = int.Parse(id);
				var courseMemberList = db.CourseMembers.Where(x => x.CourseID == CourseID);
				db.Attendances.RemoveRange(db.Attendances.Where(x => x.SessionID == SessionID));
				foreach (var item in courseMemberList)
				{
					Attendance newAttendance = new Attendance();
					newAttendance.MemberID = item.ID;
					newAttendance.SessionID = SessionID;
					newAttendance.Status = "0";
					db.Attendances.Add(newAttendance);
				}
				db.SaveChanges();

			}
			catch (Exception ex)
			{
				//catch exception if there is any
			}
			return View("QRCode", qrcode);
		}

		private string GenerateQRCode(string qrcodeText)
		{
			string folderPath = "~/Images/";
			string imagePath = "~/Images/QrCode" + qrcodeText + ".jpg";
			// create new Directory if not exist
			if (!Directory.Exists(Server.MapPath(folderPath)))
			{
				Directory.CreateDirectory(Server.MapPath(folderPath));
			}

			var barcodeWriter = new BarcodeWriter()
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Width = 400,
					Height = 400
				}
			};

			var result = barcodeWriter.Write(qrcodeText);

			string barcodePath = Server.MapPath(imagePath);
			var barcodeBitmap = new Bitmap(result);
			using (MemoryStream memory = new MemoryStream())
			{
				using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
				{
					barcodeBitmap.Save(memory, ImageFormat.Png);
					byte[] bytes = memory.ToArray();
					fs.Write(bytes, 0, bytes.Length);
				}
			}
			return imagePath;
		}
		public ActionResult ExportExcel()
		{
			int id = int.Parse(Session["CourseID"].ToString());

			//Codes for the Closed XML
			var file = Server.MapPath("~/Content/AttendanceExcelTemplate.xlsx");
			using (XLWorkbook wb = new XLWorkbook(file))
			{
				var ws = wb.Worksheet(1);
				var list = db.Sessions.Where(x => x.CourseID == id).OrderBy(x => x.Date);

				for (int j = 1; j <= 5; j++)
				{
					if (j == 1)
					{
						ws.Cell(7, j).Style.Font.Bold = true;
						ws.Cell(7, j).Style.Font.FontSize = 12;
						ws.Cell(7, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

						ws.Cell(7, j).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.TopBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.TopBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.LeftBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.RightBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.RightBorderColor = XLColor.Black;
					}
					else
					{
						ws.Cell(7, j).Style.Font.Bold = true;
						ws.Cell(7, j).Style.Font.FontSize = 12;
						ws.Cell(7, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

						ws.Cell(7, j).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.TopBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.TopBorderColor = XLColor.Black;


						ws.Cell(7, j).Style.Border.RightBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.RightBorderColor = XLColor.Black;
					}

				}
				int i = 5;
				int iIndexSession = 1;
				foreach (var item in list)
				{
					ws.Column(i).Width = 5;
					ws.Cell(7, i).Style.Font.Bold = true;
					ws.Cell(7, i).Style.Font.FontSize = 12;
					ws.Cell(7, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

					ws.Cell(7, i).Value = "B" + iIndexSession;
					ws.Cell(7, i).Comment.Style.Alignment.SetAutomaticSize();
					ws.Cell(7, i).Comment.AddText(item.Date.Value.ToShortDateString());
					ws.Cell(7, i).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.TopBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.RightBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.RightBorderColor = XLColor.Black;
					i++;
					iIndexSession++;
				}

				int iIndex = 1;
				int iCol = 1;
				int iRow = 8;
				var listStudent = db.CourseMembers.Where(x => x.CourseID == id).OrderBy(x => x.FirstName);
				foreach (var item in listStudent)
				{
					ws.Cell(iRow, iCol).Value = iIndex;
					ws.Cell(iRow, iCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
					ws.Cell(iRow, iCol).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.LeftBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.LeftBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.RightBorderColor = XLColor.Black;


					ws.Cell(iRow, iCol + 1).Value = item.StudentID;
					ws.Cell(iRow, iCol + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
					ws.Cell(iRow, iCol + 1).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 1).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 1).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.RightBorderColor = XLColor.Black;


					ws.Cell(iRow, iCol + 2).Value = item.LastName;
					ws.Cell(iRow, iCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.RightBorderColor = XLColor.Black;
					ws.Cell(iRow, iCol + 2).Value = item.LastName;
					ws.Cell(iRow, iCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.RightBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Value = item.FirstName;
					ws.Cell(iRow, iCol + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 3).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.RightBorderColor = XLColor.Black;
					for (int k = iCol; k < i; k++)
					{
						ws.Cell(iRow, k).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, k).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(iRow, k).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, k).Style.Border.TopBorderColor = XLColor.Black;

						ws.Cell(iRow, k).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, k).Style.Border.RightBorderColor = XLColor.Black;
					}
					iRow++;
					iIndex++;
				}

				string myName = Server.UrlEncode("AttendanceByExcel" + "_" + db.Courses.FirstOrDefault(x => x.ID == id).CourseName + ".xlsx");
				MemoryStream stream = GetStream(wb);// The method is defined below
				Response.Clear();
				Response.Buffer = true;
				Response.AddHeader("content-disposition", "attachment; filename=" + myName);
				Response.ContentType = "application/vnd.ms-excel";
				Response.BinaryWrite(stream.ToArray());
				Response.End();
			}

			return RedirectToAction("Index");
		}

		public ActionResult Export()
		{
			int id = int.Parse(Session["CourseID"].ToString());

			var session = db.Sessions.Where(x => x.CourseID == id).OrderBy(x => x.Date).ToList();
			var student = db.CourseMembers.Where(x => x.CourseID == id).OrderBy(x => x.FirstName).ToList();
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.FirstName = iten.FirstName;
				attendanceview.LastName = iten.LastName;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				foreach (var item in session)
				{
					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime)item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance != null)
					{
						if (attendance.Status != "0" && attendance.Status != null && attendance.Status != "")
						{
							detail.Status = "x";
						}
						else
						{
							detail.Status = "";
						}
						//if (attendance.Status !=null || attendance.Status!="")
						//{

						//	TotalPresent++;
						//	TotalPoint = TotalPoint + int.Parse(attendance.Status);
						//}
						//else
						//{
						//	detail.Status = "";
						//}
						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}
			//Codes for the Closed XML
			var file = Server.MapPath("~/Content/AttendanceExcelTemplate.xlsx");
			using (XLWorkbook wb = new XLWorkbook(file))
			{
				var ws = wb.Worksheet(1);

				for (int j = 1; j < 5; j++)
				{
					if (j == 1)
					{
						ws.Cell(7, j).Style.Font.Bold = true;
						ws.Cell(7, j).Style.Font.FontSize = 12;
						ws.Cell(7, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

						ws.Cell(7, j).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.TopBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.TopBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.LeftBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.RightBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.RightBorderColor = XLColor.Black;
					}
					else
					{
						ws.Cell(7, j).Style.Font.Bold = true;
						ws.Cell(7, j).Style.Font.FontSize = 12;
						ws.Cell(7, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

						ws.Cell(7, j).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(7, j).Style.Border.TopBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.TopBorderColor = XLColor.Black;


						ws.Cell(7, j).Style.Border.RightBorder = XLBorderStyleValues.Medium;
						ws.Cell(7, j).Style.Border.RightBorderColor = XLColor.Black;
					}

				}
				int i = 5;
				int iIndexSession = 1;
				foreach (var date in list[1].Attendance)
				{
					ws.Column(i).Width = 5;
					ws.Cell(7, i).Style.Font.Bold = true;
					ws.Cell(7, i).Style.Font.FontSize = 12;
					ws.Cell(7, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

					ws.Cell(7, i).Value = "B" + iIndexSession;
					ws.Cell(7, i).Comment.Style.Alignment.SetAutomaticSize();
					ws.Cell(7, i).Comment.AddText(date.Date.ToShortDateString());
					ws.Cell(7, i).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.TopBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.RightBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.RightBorderColor = XLColor.Black;
					i++;
					iIndexSession++;
				}
				int iIndex = 1;
				int iRow = 8;
				foreach (var item in list)
				{
					int iCol = 1;
					ws.Cell(iRow, iCol).Value = iIndex;
					ws.Cell(iRow, iCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
					ws.Cell(iRow, iCol).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.LeftBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.LeftBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol).Style.Border.RightBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 1).Value = item.studentID;
					ws.Cell(iRow, iCol + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
					ws.Cell(iRow, iCol + 1).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 1).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 1).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 1).Style.Border.RightBorderColor = XLColor.Black;


					ws.Cell(iRow, iCol + 2).Value = item.LastName;
					ws.Cell(iRow, iCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.RightBorderColor = XLColor.Black;
					ws.Cell(iRow, iCol + 2).Value = item.LastName;
					ws.Cell(iRow, iCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.RightBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Value = item.FirstName;
					ws.Cell(iRow, iCol + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 3).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 3).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 3).Style.Border.RightBorderColor = XLColor.Black;
					iCol = iCol + 4;
					foreach (var attendance in item.Attendance)
					{
						ws.Cell(iRow, iCol).Value = attendance.Status;
						ws.Cell(iRow, iCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
						ws.Cell(iRow, iCol).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, iCol).Style.Border.BottomBorderColor = XLColor.Black;

						ws.Cell(iRow, iCol).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, iCol).Style.Border.TopBorderColor = XLColor.Black;

						ws.Cell(iRow, iCol).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
						ws.Cell(iRow, iCol).Style.Border.RightBorderColor = XLColor.Black;
						iCol++;
					}
					iRow++;
					iIndex++;

				}
				string myName = Server.UrlEncode("AttendanceByExcel" + "_" + db.Courses.FirstOrDefault(x => x.ID == id).CourseName + ".xlsx");
				MemoryStream stream = GetStream(wb);// The method is defined below
				Response.Clear();
				Response.Buffer = true;
				Response.AddHeader("content-disposition", "attachment; filename=" + myName);
				Response.ContentType = "application/vnd.ms-excel";
				Response.BinaryWrite(stream.ToArray());
				Response.End();
			}

			return RedirectToAction("Index");
		}


		public MemoryStream GetStream(XLWorkbook excelWorkbook)
		{
			MemoryStream fs = new MemoryStream();
			excelWorkbook.SaveAs(fs);
			fs.Position = 0;
			return fs;
		}
		public ActionResult Import(string id)
		{
			TempData.Keep();
			return View();
		}

		[HttpPost]
		public ActionResult ReadExcel()
		{
			List<DasboardAttendanceView> listAttendance = new List<DasboardAttendanceView>();
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
							string path = Server.MapPath("~/Uploads/");
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							filePath = path + Path.GetFileName(fileName);
							file.SaveAs(filePath);

							using (XLWorkbook wb = new XLWorkbook(filePath))
							{
								var ws = wb.Worksheet(1);
								int iMaxCol = 1;

								for (int i = 1; i < int.MaxValue; i++)
								{
									if (ws.Cell(7, i).IsEmpty())
									{
										iMaxCol = i - 1;
										break;
									}
								}
								for (int i = 8; i < int.MaxValue; i++)
								{
									if (ws.Cell(i, 1).IsEmpty())
									{
										break;
									}
									DasboardAttendanceView attendanceview = new DasboardAttendanceView();
									attendanceview.studentID = ws.Cell(i, 2).GetValue<string>();
									attendanceview.LastName = ws.Cell(i, 3).GetValue<string>();
									attendanceview.FirstName = ws.Cell(i, 4).GetValue<string>();
									for (int j = 5; j <= iMaxCol; j++)
									{
										DetailAttendance detail = new DetailAttendance();
										detail.Date = DateTime.Parse(ws.Cell(7, j).Comment.Text);
										detail.Note = ws.Cell(i, j).Comment.Text;
										//TO DO
										if (ws.Cell(i, j).GetValue<string>().ToLower() == "x")
										{
											detail.Status = "10";
										}
										else
										{
											detail.Status = "0";
										}

										attendanceview.Attendance.Add(detail);
									}
									listAttendance.Add(attendanceview);
								}
							}
							//delete the file from physical path after reading 
							string filedetails = path + fileName;
							FileInfo fileinfo = new FileInfo(filedetails);
							if (fileinfo.Exists)
							{
								fileinfo.Delete();
							}
							//delete session not attendance

							TempData["AttendanceExcel"] = listAttendance.OrderBy(x => x.FirstName).ToList();

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
		public ActionResult InsertExcelData()
		{
			int CourseID = int.Parse(Session["CourseID"].ToString());
			try
			{
				if (TempData["AttendanceExcel"] != null)
				{
					List<DasboardAttendanceView> listAttendance = (List<DasboardAttendanceView>)TempData["AttendanceExcel"];
					using (var scope = new TransactionScope())
					{
						var listSession = db.Sessions.Where(x => x.CourseID == CourseID);
						foreach (var item in listSession)
						{
							db.Attendances.RemoveRange(db.Attendances.Where(x => x.SessionID == item.ID));
						}
						db.SaveChanges();

						db.Sessions.RemoveRange(listSession);
						db.SaveChanges();

						foreach (var item in listAttendance[1].Attendance)
						{
							Session nSession = new Session();
							nSession.Date = item.Date;
							nSession.CourseID = CourseID;
							db.Sessions.Add(nSession);
						}
						db.SaveChanges();
						foreach (var item in listAttendance)
						{
							var MemberID = db.CourseMembers.FirstOrDefault(x => x.StudentID == item.studentID && x.CourseID == CourseID).ID;

							foreach (var aItem in item.Attendance)
							{
								var SessionID = db.Sessions.FirstOrDefault(x => x.Date == aItem.Date && x.CourseID == CourseID).ID;
								Attendance nAttendance = new Attendance();
								nAttendance.MemberID = MemberID;
								nAttendance.SessionID = SessionID;
								nAttendance.Status = aItem.Status;
								nAttendance.Note = aItem.Note;
								db.Attendances.Add(nAttendance);
							}
						}
						db.SaveChanges();
						scope.Complete();
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			TempData.Remove("AttendanceExcel");
			Session["tabactive"] = "tab1";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		public ActionResult CheckByCode()
		{
			var student = db.CourseMembers.FirstOrDefault(x => x.Email == User.Identity.Name);
			if (student == null)
			{
				HttpCookie cookieNotStudent = new HttpCookie("notStudent", "true");
				HttpContext.Response.Cookies.Add(cookieNotStudent);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return View(student);
			}

		}
		[HttpPost]
		public string CheckByCode(string id)
		{
			if (AttendanceList.Contains(id))
			{
				return "true";
			}
			else
			{
				return "false";
			}
		}
		[HttpPost]
		public ActionResult CheckAttendanceQRcode(CheckQRCodeModel model)
		{
			int QrCodeID = int.Parse(model.QRcode.ToString());
			var session = db.Sessions.FirstOrDefault(x => x.ID == QrCodeID);
			var sessionID = session.ID;
			var member = db.CourseMembers.FirstOrDefault(x => x.StudentID == model.StudentID && x.CourseID == session.CourseID);
			if (member != null)
			{
				var memberID = member.ID;
				using (var scope = new TransactionScope())
				{
					var Attendance = db.Attendances.FirstOrDefault(x => x.MemberID == memberID && x.SessionID == sessionID);
					if (Attendance == null)
					{
						Attendance newAttendance = new Attendance();
						newAttendance.MemberID = memberID;
						newAttendance.Picture = model.PictureBase64;
						newAttendance.SessionID = sessionID;
						newAttendance.Status = "10";
						db.Attendances.Add(newAttendance);
						db.SaveChanges();
					}
					else
					{
						Attendance.MemberID = memberID;
						Attendance.Picture = model.PictureBase64;
						Attendance.SessionID = sessionID;
						Attendance.Status = "10";
						db.SaveChanges();
					}
					scope.Complete();
				}
				HttpCookie cookieCheckAttendanceSuccess = new HttpCookie("cookieCheckAttendanceSuccess", "true");
				HttpContext.Response.Cookies.Add(cookieCheckAttendanceSuccess);
				return RedirectToAction("CheckByCode");
			}
			else
			{

				HttpCookie cookieNotStudentInCourse = new HttpCookie("cookieNotStudentInCourse", "true");
				HttpContext.Response.Cookies.Add(cookieNotStudentInCourse);
				return RedirectToAction("CheckByCode");
			}
		}

		[HttpPost]
		public String CloseAttendance(string id)
		{
			string folderPath = "~/Images/";
			var serverPath = HttpContext.Server.MapPath(folderPath);
			if (Directory.Exists(Path.GetDirectoryName(serverPath)) == true)
			{
				var fileEntries = Directory.GetFiles(serverPath);
				if (fileEntries != null)
				{
					foreach (var fileEntry in fileEntries)
					{
						{
							System.IO.File.Delete(fileEntry);
						}
					}
				}
			}
			AttendanceList.Remove(id);
			return "true";
		}

		[HttpGet]
		public JsonResult GetAttendanceData()
		{
			int CourseID = int.Parse(Session["CourseID"].ToString());
			var listSession = db.Sessions.Where(x => x.CourseID == CourseID).OrderBy(x => x.Date);
			List<string> itemList = new List<string>();
			foreach (var item in listSession)
			{
				var attendanceCount = item.Attendances.Where(x => x.Status != "0").Count();
				itemList.Add(attendanceCount.ToString());
			}
			return Json(itemList, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getMaxStudent()
		{
			int CourseID = int.Parse(Session["CourseID"].ToString());
			var maxStudent = db.CourseMembers.Where(x => x.CourseID == CourseID).Count();
			return Json(maxStudent, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public ActionResult ViewDetailAttendance(string id)
		{
			var courseID = int.Parse(Session["CourseID"].ToString());

			int SessionID = int.Parse(id.ToString());

			var attendance = db.Attendances.Where(x => x.SessionID == SessionID);

			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.FirstName).ToList();
			List<EditAttendanceView> list = new List<EditAttendanceView>();
			foreach (var item in student)
			{
				EditAttendanceView newEditView = new EditAttendanceView();
				newEditView.MemberID = item.ID;
				newEditView.DoB = item.DoB;
				newEditView.StudentID = item.StudentID;
				newEditView.StudentName = item.LastName + item.FirstName;

				if (attendance.Count() > 0)
				{
					var detailAttendance = attendance.FirstOrDefault(x => x.MemberID == item.ID);
					if (detailAttendance != null)
					{
						newEditView.Date = attendance.First().Session.Date;
						newEditView.Note = attendance.FirstOrDefault(x => x.MemberID == item.ID).Note;
						newEditView.Picture = attendance.FirstOrDefault(x => x.MemberID == item.ID).Picture;
						newEditView.Status = attendance.FirstOrDefault(x => x.MemberID == item.ID).Status;
					}

				}
				list.Add(newEditView);
			}
			return View(list);
		}

		[HttpGet]
		public ActionResult ViewEditAttendance(string id)
		{
			var courseID = int.Parse(Session["CourseID"].ToString());

			int SessionID = int.Parse(id.ToString());

			var attendance = db.Attendances.Where(x => x.SessionID == SessionID);

			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.FirstName).ToList();
			List<EditAttendanceView> list = new List<EditAttendanceView>();
			foreach (var item in student)
			{
				EditAttendanceView newEditView = new EditAttendanceView();
				newEditView.MemberID = item.ID;
				newEditView.DoB = item.DoB;
				newEditView.StudentID = item.StudentID;
				newEditView.StudentName = item.LastName + item.FirstName;

				if (attendance.Count() > 0)
				{
					var detailAttendance = attendance.FirstOrDefault(x => x.MemberID == item.ID);
					if (detailAttendance != null)
					{
						newEditView.Date = attendance.First().Session.Date;
						newEditView.Note = attendance.FirstOrDefault(x => x.MemberID == item.ID).Note;
						newEditView.Picture = attendance.FirstOrDefault(x => x.MemberID == item.ID).Picture;
						newEditView.Status = attendance.FirstOrDefault(x => x.MemberID == item.ID).Status;
					}

				}
				list.Add(newEditView);
			}
			return View(list);
		}
		public ActionResult EditAttendance()
		{
			return View();

		}

	}
}