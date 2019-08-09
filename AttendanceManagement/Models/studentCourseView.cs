using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class studentCourseView
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public studentCourseView()
		{
			this.attendanceList = new HashSet<DetailAttendance>();
		}
		public string studentName { get; set; }
		public string studentID { get; set; }
		public DateTime studentDoB { get; set; }
		public string courseName { get; set; }

		public int totalSession { get; set; }
		public int attendanceCount { get; set; }
		public int attendancePoint { get; set; }


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<DetailAttendance> attendanceList { get; set; }
	}
}