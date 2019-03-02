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
    }
}
