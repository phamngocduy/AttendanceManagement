using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoginManagement;
using LoginManagement.Models;
using System.Web;
using IdentificationManagement.Controllers;

namespace LoginManagement.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {

        [TestMethod]
        public void TestViewEditProfile()
        {
            var controller = new AccountController();
            string id = "1";

            var viewResult = controller.Edit(id) as ViewResult;

            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void TestEditProfileSuccessfully()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");           
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();           
            edit.PhoneNumber = "924942472";
            edit.DoB = dt;           
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as ViewResult;
            Assert.IsTrue(edit.PhoneNumber.ToString().Equals("924942472"));
            Assert.IsTrue(edit.DoB.ToString().Equals("11/23/2010"));         
            Assert.IsNotNull(result1);
            System.Diagnostics.Trace.WriteLine("Udate profile succesully");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithEmptyPhoneNumber()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = "";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid phone number");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithBlankOfBirth()
        {

            //Arange
            DateTime dt = Convert.ToDateTime(null);
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = "924942472";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid day of birthh");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithhSpaceOfPhhoneNumber()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = " ";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid phone number");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithhSpaceOfDoB()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = "andignoieo";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid phone number");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithASpaceOfPhoneNumber()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = "924 942472";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid phone number");
        }

        [TestMethod]
        public void TestEditProfileUnSuccessfullyWithSpecialLetterOfPhoneNumber()
        {

            //Arange
            DateTime dt = Convert.ToDateTime("11/23/2010");
            var controller = new AccountController();
            var user = new User();
            var db = new cap21t4Entities();
            User edit = db.Users.First();
            edit.PhoneNumber = "#%&*^%#*@";
            edit.DoB = dt;
            var result1 = controller.Edit(db.Groups.First().ID.ToString("3")) as RedirectResult;
            Assert.IsNull(result1);
            System.Diagnostics.Trace.WriteLine("Please enter a valid phone number");
        }
             
    }

}
