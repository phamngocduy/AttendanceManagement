using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class DetailAttendance
	{
		public DateTime Date { get; set; }
		public string Status { get; set; }

		public string Note { get; set; }
	}
}