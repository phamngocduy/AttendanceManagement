using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
using WebApplication.Controllers;
using WebApplication.Models;
using System.Transactions;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class GroupControllerTest
    {
        [TestMethod]
        public void TestViewMyGroup()
        {
            var controller = new GroupController();
            var result = controller.Index() as ViewResult;
            var db = new cap21t4Entities();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<Group>));
            Assert.AreEqual(db.Groups.Where(x => x.GroupParent == null).Count(), ((List<Group>)result.Model).Count);
        }
        [TestMethod]
        public void TestViewEditGroup()
        {
            var controller = new GroupController();
            string id = "1";

            var viewResult = controller.Edit(id) as ViewResult;

            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void TestEditGroupSuccessfully()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

            }

        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWitEmptyName()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                //Assert.IsNotNull(result1);
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }
        }

        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithSpecialSymbolName()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "&^%$%$";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

                System.Diagnostics.Trace.WriteLine("Error data format, please try again");

            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithBlankSpaceName()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = " ";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

                System.Diagnostics.Trace.WriteLine("Error data format, please try again");

            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithEmptyDescription()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

                System.Diagnostics.Trace.WriteLine("Error data format, please try again");

            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithSymbolDescription()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "&^%$%$";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

                System.Diagnostics.Trace.WriteLine("Error data format, please try again");

            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithBlankSpaceDescription()
        {

            //Arange
            var controller = new GroupController();
            var user = new Group();
            var db = new cap21t4Entities();
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = " ";

            using (var scope = new TransactionScope())
            {

                var result1 = controller.Edit(db.Groups.First().ID.ToString()) as ViewResult;
                Assert.IsNotNull(result1);

                System.Diagnostics.Trace.WriteLine("Error data format, please try again");

            }
        }
    }
}

    

