using JWT_BASICS.Controllers;
using JWT_BASICS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Newtonsoft.Json;

namespace JWT_BASICS.Test
{
    [TestClass]
    public class LoginExampleControllerTest
    {
        [TestMethod]
        public void loginwithoutclaimsTest_UnAuthorized()
        {
            // Prepare
            string userId = "1";
            string password = "2";
            var configuration = Global.appConfiguration;
            var jwt = new JWT(configuration);
            var loginExampleController = new LoginExampleController(jwt);

            //Execution
            
            ResponseAuthentication result = loginExampleController.loginwithoutclaims(userId, password);
            
            // Verify            
            Assert.AreEqual(401, result.HttpStatusCode);
        }

        [TestMethod]
        public void loginwithoutclaimsTest_EmptyRequest()
        {
            // Prepare
            string userId = "";
            string password = "";
            var configuration = Global.appConfiguration;
            var jwt = new JWT(configuration);
            var loginExampleController = new LoginExampleController(jwt);

            //Execution

            ResponseAuthentication result = loginExampleController.loginwithoutclaims(userId, password);

            // Verify            
            Assert.AreEqual(401, result.HttpStatusCode);
        }

        [TestMethod]
        public void loginwithoutclaimsTest_Authorized()
        {
            // Prepare

            var configuration = Global.appConfiguration;
            var jwt = new JWT(configuration);
            var loginExampleController = new LoginExampleController(jwt);

            //Execution

            ResponseAuthentication result = loginExampleController.loginwithoutclaims("1","1");

            // Verify
            
            Assert.AreEqual(200, result.HttpStatusCode);
        }
    }
}