using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Extension;
using System.Web.Routing;
using System.Web;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class LocalizedControllerActivatorTest
    {
        [TestMethod]
        public void TestMultipleLanguage()
        {
            // Arrange
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost:54325", null), new HttpResponse(null));
            LocalizedControllerActivator IControllerActivator = new LocalizedControllerActivator();

            var requestContext = HttpContext.Current.Request.RequestContext;
            var controllerType = HttpContext.Current.Request.RequestType.GetType();
            // Act
            IController result = IControllerActivator.Create(requestContext, controllerType) as IController;
            //Assert
            Assert.IsNull(result);
        }
    }
}
