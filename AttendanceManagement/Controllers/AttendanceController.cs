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

namespace AttendanceManagement.Controllers
{
	public class AttendanceController : Controller
	{
		AttendanceEntities db = new AttendanceEntities();
		APIController api = new APIController();

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
		public ActionResult DetailClass(string id)
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
			if (Session["SessionID"] != null)
			{
				int sessionid = int.Parse(Session["SessionID"].ToString());
				DateTime? date = db.Sessions.FirstOrDefault(x => x.ID == sessionid).Date;
				TempData["date"] = date.Value.ToShortDateString();
				Session.Remove("SessionID");
			}
			var session = db.Sessions.Where(x => x.CourseID == courseID).OrderBy(x => x.Date).ToList();
			ViewData["session"] = session;
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).OrderBy(x => x.Name).ToList();
			ViewData["students"] = student;
			List<DasboardAttendanceView> list = new List<DasboardAttendanceView>();
			foreach (var iten in student)
			{
				DasboardAttendanceView attendanceview = new DasboardAttendanceView();
				attendanceview.memberID = iten.ID;
				attendanceview.studentID = iten.StudentID;
				attendanceview.studentName = iten.Name;
				attendanceview.TotalSession = session.Count();
				int TotalPresent = 0;
				int TotalPoint = 0;
				List<DetailAttendance> listAttendance = new List<DetailAttendance>();
				foreach (var item in session)
				{

					DetailAttendance detail = new DetailAttendance();
					detail.Date = (DateTime)item.Date;
					var attendance = item.Attendances.FirstOrDefault(x => x.MemberID == iten.ID);
					if (attendance == null)
					{
						detail.Status = "N/A";
						detail.Note = "";
					}
					else
					{
						detail.Status = attendance.Status;

						if (attendance.Status != "0")
						{
							TotalPresent++;
						}
						TotalPoint = TotalPoint + int.Parse(attendance.Status);
						detail.Note = attendance.Note;
					}
					attendanceview.Attendance.Add(detail);
					listAttendance.Add(detail);
				}
				attendanceview.TotalPoint = TotalPoint;
				attendanceview.TotalPresent = TotalPresent;
				list.Add(attendanceview);
			}

			ViewData["attendance"] = list;
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
		public ActionResult AddStudent(string groupname)
		{
			string groupdata = api.ReadData("http://localhost:54325/api/getAllGroups");
			List<GroupModel> group = JsonConvert.DeserializeObject<List<GroupModel>>(groupdata);
			ViewBag.Group = group;
			if (groupname != null)
			{
				string userdata = api.ReadData("http://localhost:54325/api/getMember?groupname=" + groupname);
				List<UserModel> user = JsonConvert.DeserializeObject<List<UserModel>>(userdata);
				return View(user);
			}

			return View();

		}
		[HttpPost]
		public ActionResult AddStudent(List<string> studentlist)
		{
			foreach (var item in studentlist)
			{
				string studentdata = api.ReadData("http://localhost:54325/api/getUserInfo?searchString=" + item);
				UserModel student = JsonConvert.DeserializeObject<UserModel>(studentdata);
				var courseID = int.Parse(Session["CourseID"].ToString());
				var course = db.Courses.FirstOrDefault(x => x.ID == courseID);
				var StudentInCourse = course.CourseMembers.FirstOrDefault(x => x.StudentID == student.StID);
				if (StudentInCourse == null)
				{
					CourseMember newMember = new CourseMember();
					newMember.CourseID = courseID;
					newMember.StudentID = student.StID;
					newMember.Name = student.FullName;
					newMember.Email = student.Email;
					newMember.DoB = student.DoB;
					newMember.Avatar = student.Avatar;
					course.CourseMembers.Add(newMember);
				}
			}
			db.SaveChanges();
			Session["tabactive"] = "tab2";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		public ActionResult FacultyIndex()
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
			//	ViewBag.GroupMap = groupdb.Groups.Where(x => x.GroupParent != null).ToList();
			return View();
		}
		[HttpPost]
		public ActionResult CreateFaculty1(Faculty faculty)
		{
			Faculty newfaculty = new Faculty();
			newfaculty.Name = faculty.Name;
			newfaculty.Description = faculty.Description;
			newfaculty.GroupID = faculty.GroupID;
			db.Faculties.Add(newfaculty);
			db.SaveChanges();
			return RedirectToAction("FacultyIndex");
		}
		[HttpGet]
		public ActionResult CheckAttendance(string id)
		{
			Session["SessionID"] = id;
			Session["tabactive"] = "tab4";

			return RedirectToAction("DetailClass", "Attendance", new { id = Session["CourseID"] });
		}

		[HttpPost]
		public JsonResult CheckAttendance(List<AttendanceModel> attendance)
		{
			using (var scope = new TransactionScope())
			{
				int sessionID = int.Parse(Session["SessionID"].ToString());
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
				TempData.Remove("date");
				Session.Remove("SessionID");
				Session["tabactive"] = "tab1";
			}

			return Json("true");
		}
		public ActionResult TestQR()
		{
			return View();
		}
		[HttpPost]
		public JsonResult ClearYourSessionValue()
		{
			// This will leave the key in the Session cache, but clear the value
			Session["SessionQRID"] = null;

			// This will remove both the key and value from the Session cache
			Session.Remove("SessionQRID");

			return Json("revove session");

		}

		[HttpPost]
		public string CheckSessionQRCode()
		{
			if (Session["SessionQRID"] != null)
			{
				return "true";
			}
			else
			{
				return "false";
			}



		}
		public ActionResult Generate(string id)
		{
			Session["SessionQRID"] = id;
			QRCodeModel qrcode = new QRCodeModel();
			qrcode.QRCodeText = id;
			try
			{
				qrcode.QRCodeImagePath = GenerateQRCode(id);
				ViewBag.Message = "QR Code Generated successfully";
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
			string imagePath = "~/Images/QrCode.jpg";
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
				foreach (var item in list)
				{

					ws.Cell(7, i).Style.Font.Bold = true;
					ws.Cell(7, i).Style.Font.FontSize = 12;
					ws.Cell(7, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

					ws.Cell(7, i).Value = "'" + item.Date.Value.ToShortDateString();
					ws.Cell(7, i).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.TopBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(7, i).Style.Border.RightBorder = XLBorderStyleValues.Medium;
					ws.Cell(7, i).Style.Border.RightBorderColor = XLColor.Black;
					i++;
				}
				int iIndex = 1;
				int iCol = 1;
				int iRow = 8;
				var listStudent = db.CourseMembers.Where(x => x.CourseID == id).OrderBy(x => x.Name);
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


					ws.Cell(iRow, iCol + 2).Value = item.Name;
					ws.Cell(iRow, iCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.BottomBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.TopBorderColor = XLColor.Black;

					ws.Cell(iRow, iCol + 2).Style.Border.RightBorder = XLBorderStyleValues.Dotted;
					ws.Cell(iRow, iCol + 2).Style.Border.RightBorderColor = XLColor.Black;
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


		public MemoryStream GetStream(XLWorkbook excelWorkbook)
		{
			MemoryStream fs = new MemoryStream();
			excelWorkbook.SaveAs(fs);
			fs.Position = 0;
			return fs;
		}
		//public ActionResult CheckAttendanceByCode()
		//{

		//}


	}
}