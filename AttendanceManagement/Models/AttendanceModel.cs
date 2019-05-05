using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class AttendanceModel
	{
		public string studentID { get; set; }

		public int memberID { get; set; }

		public string status { get; set; }

		public string note { get; set; }
	
	}
}