using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class CourseModel
	{
		public int Semester { get; set; }
		public List<CourseData> Courses { get; set; }
		
	}
}