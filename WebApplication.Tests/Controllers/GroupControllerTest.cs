using System;
using System.IO;
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
using MvcContrib.TestHelper;
using Moq;
using System.Web;
using System.Web.Routing;

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
                Assert.IsTrue(edit.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals("Mobile App"));
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
                Assert.IsTrue(edit.GroupName.ToString().Equals(""));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals("Mobile App"));
                Assert.IsNotNull(result1);
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
                Assert.IsTrue(edit.GroupName.ToString().Equals("&^%$%$"));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals("Mobile App"));
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
                Assert.IsTrue(edit.GroupName.ToString().Equals(" "));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals("Mobile App"));
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
                Assert.IsTrue(edit.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals(""));
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
                Assert.IsTrue(edit.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(edit.GroupDescription.ToString().Equals("&^%$%$"));
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
        [TestMethod]
        public void TestDetailOfAGroup()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            var controller = new GroupController();
            builder.InitializeController(controller);
            var result = controller.Detail("3") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestReadExcel()
        {
            var controller = new GroupController();
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var files = new Mock<HttpFileCollectionBase>();
            var file = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            context.Setup(c => c.Request).Returns(request.Object);
            request.Setup(r => r.Files).Returns(files.Object);
            files.Setup(f => f["file"]).Returns(file.Object);
            context.Setup(c => c.Server).Returns(server.Object);
            file.Setup(f => f.ContentLength).Returns(1);
            file.Setup(f => f.ContentType).Returns("Excel");
            server.Setup(s => s.MapPath("~/Uploads/")).Returns("./Uploads/");

            var case1 = "ReadExcel.xls";
            file.Setup(f => f.FileName).Returns(case1);
            var reader = new StreamReader(case1);
            file.Setup(f => f.InputStream).Returns(reader.BaseStream);
            var result = controller.ReadExcel() as JsonResult;
            Assert.IsInstanceOfType(result.Data, typeof(List<User>));
            Assert.AreEqual(0, ((List<User>)result.Data).Count);

            var case2 = "ReadExcel.xlsx";
            file.Setup(f => f.FileName).Returns(case2);
            reader = new StreamReader(case2);
            file.Setup(f => f.InputStream).Returns(reader.BaseStream);
            result = controller.ReadExcel() as JsonResult;
            Assert.IsInstanceOfType(result.Data, typeof(List<User>));
            Assert.AreEqual(0, ((List<User>)result.Data).Count);
        }

        [TestMethod]
        public void TestReadExcelUnSeccess()
        {
            var controller = new GroupController();
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var files = new Mock<HttpFileCollectionBase>();
            var file = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            context.Setup(c => c.Request).Returns(request.Object);
            request.Setup(r => r.Files).Returns(files.Object);
            files.Setup(f => f["file"]).Returns(file.Object);
            context.Setup(c => c.Server).Returns(server.Object);
            file.Setup(f => f.ContentLength).Returns(1);
            file.Setup(f => f.ContentType).Returns("Excel");
            server.Setup(s => s.MapPath("~/Uploads/")).Returns("./Uploads/");

            var case3 = "Word.docx";
            file.Setup(f => f.FileName).Returns(case3);
            var reader = new StreamReader(case3);
            file.Setup(f => f.InputStream).Returns(reader.BaseStream);
            var result = controller.ReadExcel() as ViewResult;
            System.Diagnostics.Trace.WriteLine("This file format is not supported");
        }

        [TestMethod]
        public async Task TestInsertExcelSucess()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            GroupController controller = new GroupController();
            builder.InitializeController(controller);
            var db = new cap21t4Entities();
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var files = new Mock<HttpFileCollectionBase>();
            var file = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var session = new Mock<HttpSessionStateBase>();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            context.Setup(c => c.Request).Returns(request.Object);
            request.Setup(r => r.Files).Returns(files.Object);
            files.Setup(f => f["file"]).Returns(file.Object);
            context.Setup(c => c.Server).Returns(server.Object);
            file.Setup(f => f.ContentLength).Returns(1);
            file.Setup(f => f.ContentType).Returns("Excel");
            server.Setup(s => s.MapPath("~/Uploads/")).Returns("./Uploads/");
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            await controller.InsertExcelData();

        }
    }
}

    

