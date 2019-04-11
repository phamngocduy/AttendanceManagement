//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            this.Sessions = new HashSet<Session>();
            this.Members = new HashSet<Member>();
        }
    
        public int ID { get; set; }
        public string CourseName { get; set; }
        public string Type { get; set; }
        public string Major { get; set; }
        public string Lecturer { get; set; }
        public Nullable<int> Credit { get; set; }
        public Nullable<int> Students { get; set; }
        public string DayOfWeek { get; set; }
        public Nullable<System.TimeSpan> TimeSpan { get; set; }
        public Nullable<int> Periods { get; set; }
        public string Room { get; set; }
        public string Note { get; set; }
    
        public virtual Major Major1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member> Members { get; set; }
    }
}
