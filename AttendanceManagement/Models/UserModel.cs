﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
	public class UserModel
	{
		public string StID { get; set; }
		public string FullName { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public Nullable<System.DateTime> DoB { get; set; }
		public string UserID { get; set; }
		public string Avatar { get; set; }

	}
}