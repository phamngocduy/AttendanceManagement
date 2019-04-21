using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class FacultyViewModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string GroupLink { get; set; }
		public string Description { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Major> Majors { get; set; }
	}
}