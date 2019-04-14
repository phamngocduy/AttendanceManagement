using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
	public class CourseModel
	{
		public string Semester { get; set; }
		public List<CourseData> Courses { get; set; }
		
	}
}