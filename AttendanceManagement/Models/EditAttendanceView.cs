using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class EditAttendanceView
	{
		public Nullable<System.DateTime> Date { get; set; }
		public string Status { get; set; }
		public string StudentName { get; set; }
		public string StudentID { get; set; }
		public int MemberID { get; set; }
		public	Nullable<System.DateTime> DoB { get; set; }
		public string Note { get; set; }
		public string Picture { get; set; }
	}
}