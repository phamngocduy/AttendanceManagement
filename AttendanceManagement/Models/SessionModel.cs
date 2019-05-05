using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class SessionModel
	{
		public int TotalWeek { get; set; }
		public Nullable<System.DateTime> StartDate { get; set; }
		public Nullable<System.TimeSpan> Time { get; set; }

	}
}