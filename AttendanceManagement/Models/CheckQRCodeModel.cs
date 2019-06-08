using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class CheckQRCodeModel
	{
		public string StudentID { get; set; }
		public string QRcode { get; set; }
		public string PictureBase64 { get; set; }
	}
}