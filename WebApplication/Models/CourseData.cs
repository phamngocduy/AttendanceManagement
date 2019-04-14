using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
	public class CourseData
	{


		public string Code { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Major { get; set; }
		public string Lecturer { get; set; }
		public string Credit { get; set; }
		public int Students { get; set; }
		public string DayOfWeek { get; set; }
		public Nullable<System.TimeSpan> TimeSpan { get; set; }
		public int Periods { get; set; }
		public string Room { get; set; }
		public Notedata Note { get; set; }
	}
	public class Notedata
	{
		public string MaLop { get; set; }
		public string TenLop { get; set; }

	}
}
