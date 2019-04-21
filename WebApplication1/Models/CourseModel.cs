using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class CourseModel
	{
		public int Semester { get; set; }
		public List<CourseData> Courses { get; set; }
		
	}
}