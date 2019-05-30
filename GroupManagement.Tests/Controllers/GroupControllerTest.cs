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
using System.Threading.Tasks;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class GroupControllerTest
    {
        [TestMethod]
        public void TestViewMyGroup()
        {
            var context = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            context.Setup(t => t.User).Returns(principal);
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(context.Object);
            var controller = new GroupController();

            //controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.ControllerContext = controllerContext.Object;
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
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals("Mobile App"));

            }

        }

        [TestMethod]
        public void TestEditGroupUnSuccessfullyWitEmptyName()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals(""));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals("Mobile App"));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }
        }



        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithSpecialSymbolName()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "&^%$%$";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals("&^%$%$"));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals("Mobile App"));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithBlankSpaceName()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = " ";
            edit.GroupDescription = "Mobile App";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals(" "));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals("Mobile App"));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }
        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithEmptyDescription()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals(""));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }

        }
        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithSymbolDescription()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = "&^%$%$";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals("&^%$%$"));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }

        }

        [TestMethod]
        public void TestEditGroupUnSuccessfullyWithBlankSpaceDescription()
        {
            //Arange
            var controller = new GroupController();
            var db = new cap21t4Entities();
            //mock .Identity.Name
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;
            //new group
            Group edit = db.Groups.First();
            edit.GroupName = "Nhóm 1.5";
            edit.GroupDescription = " ";

            using (var scope = new TransactionScope())
            {
                var result = controller.Edit(edit) as RedirectToRouteResult;
                Assert.AreEqual(result.RouteValues["action"], "Index");

                var newGroup = db.Groups.FirstOrDefault(x => x.ID == edit.ID);
                Assert.IsTrue(newGroup.GroupName.ToString().Equals("Nhóm 1.5"));
                Assert.IsTrue(newGroup.GroupDescription.ToString().Equals(" "));
                System.Diagnostics.Trace.WriteLine("Error data format, please try again");
            }
        }

        [TestMethod]
        public void TestViewDeleteGroup()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            var controller = new GroupController();
            builder.InitializeController(controller);
            var result = controller.Delete("76") as ViewResult;
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
        [TestMethod]
        public void TestAddGroupOwner()
        {
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            GroupController controller = new GroupController();
            builder.InitializeController(controller);
            var controllerContext = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            //get session        
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            controllerContext.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            controllerContext.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = controllerContext.Object;
            var group = new Group();
            var db = new cap21t4Entities();
            using (var scope = new TransactionScope())
            {
                var result1 = controller.AddGroupOwner(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }

        }
        [TestMethod]
        public void TestAddGroupManager()
        {
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            GroupController controller = new GroupController();
            builder.InitializeController(controller);
            var controllerContext = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            //get session        
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            controllerContext.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            controllerContext.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = controllerContext.Object;
            var group = new Group();
            var db = new cap21t4Entities();
            using (var scope = new TransactionScope())
            {
                var result1 = controller.AddGroupManager(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }

        }
        [TestMethod]
        public void TestCreateGroupSuccessfully()
        {
            //Arange
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            Group create = db.Groups.First();
            create.ID = 999;
            create.GroupName = "Cap team 4";
            create.GroupType = false;
            create.GroupDescription = "test";
            create.GroupParent = 76;
            create.CreatedDate = DateTime.Now;
            using (var scope = new TransactionScope())
            {
                var result1 = controller.CreateGroup(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }
        }
        [TestMethod]
        public void TestCreateGroupUnSuccessfullyWitEmptyName()
        {
            //Arange
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            Group create = db.Groups.First();
            create.ID = 999;
            create.GroupName = "";
            create.GroupType = false;
            create.GroupDescription = "test";
            create.GroupParent = 76;
            create.CreatedDate = DateTime.Now;
            using (var scope = new TransactionScope())
            {
                var result1 = controller.CreateGroup(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }
        }

        [TestMethod]
        public void TestCreateGroupUnSuccessfullyWithSpecialSymbolName()
        {
            //Arange
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            Group create = db.Groups.First();
            create.ID = 999;
            create.GroupName = "&^%$%$";
            create.GroupType = false;
            create.GroupDescription = "test";
            create.GroupParent = 76;
            create.CreatedDate = DateTime.Now;
            using (var scope = new TransactionScope())
            {
                var result1 = controller.CreateGroup(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }
        }
        [TestMethod]
        public void TestCreateGroupUnSuccessfullyWithBlankSpaceName()
        {
            //Arange
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            Group create = db.Groups.First();
            create.ID = 999;
            create.GroupName = " ";
            create.GroupType = false;
            create.GroupDescription = "test";
            create.GroupParent = 76;
            create.CreatedDate = DateTime.Now;
            using (var scope = new TransactionScope())
            {
                var result1 = controller.CreateGroup(db.Groups.First().ID.ToString("1")) as RedirectResult;
                Assert.IsNull(result1);
            }
        }
        [TestMethod]
        public void TestDetailOfAGroup()
        {
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            var result = controller.Detail("76") as ViewResult;
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestViewCreateGroup()
        {
            GroupController controller = new GroupController();
            var user = new User();
            var db = new cap21t4Entities();
            var group = new Group();

            //session
            var GroupID = 1;
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            var mock = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            mock.Setup(ctx => ctx.HttpContext).Returns(mockHttpContext.Object);
            mock.Setup(p => p.HttpContext.Session["GroupID"]).Returns(GroupID);
            controller.ControllerContext = mock.Object;

            //Identity name           
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("duykhau1@gmail.com");
            controller.ControllerContext = mock.Object;

            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestViewAddGroupManager()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            var controller = new GroupController();
            builder.InitializeController(controller);
            var result = controller.AddGroupManager() as ViewResult;
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestViewAddGroupOwner()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            var controller = new GroupController();
            builder.InitializeController(controller);
            var result = controller.AddGroupOwner() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}


    

