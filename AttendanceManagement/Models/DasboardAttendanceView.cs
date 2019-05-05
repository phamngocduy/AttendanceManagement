using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class DasboardAttendanceView
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public DasboardAttendanceView()
		{
			this.Attendance = new HashSet<DetailAttendance>();
		}
		public int memberID { get; set; }
		public string studentID { get; set; }
		public string studentName { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<DetailAttendance> Attendance { get; set; }
	
	}
}