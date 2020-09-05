namespace MicroCredentialUnitTestProject.ControllerTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MicroCredential.Models;
    using MicroCredential.Controllers;
    using NUnit.Framework;
    using Moq;
    using MicroCredential.Services;
    using MicroCredential.Properties;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System.Linq;

    public class TokenControllerTest
    {
        [Test]
        public void AsExpectedCustomerDashboardDetails()
        {
            var mockService = new Mock<ICosmosDatabaseService>();
            var controller = new TokenController(mockService.Object);

            List<CustomerDashboard> customerDashboards = getCustomerDaashboards();

            mockService.Setup(x => x.GetCustomerDashboards()).Returns(customerDashboards);

            var actual = controller.Get();

            Assert.NotNull(actual);
            Assert.AreEqual(customerDashboards.Count, actual.Count());
        }

        [Test]
        public async Task AsExpectedWhenPostIsFailed()
        {
            var mockService = new Mock<ICosmosDatabaseService>();

            mockService.Setup(x => x.AddCustomerDetails(It.IsAny<CustomerDetails>())).Returns(Task.FromResult("MC183939"));

            var controller = new TokenController(mockService.Object);
            controller.ModelState.AddModelError("ServiceType", "Required");

            var customerDetails = new CustomerDetails
            {
                CustomerName = "Raj"
            };

            var actual = await controller.Post(customerDetails) as ObjectResult;

            Assert.AreEqual(StatusCodes.Status400BadRequest, actual.StatusCode);

        }

        [Test]
        public async Task AexpectedResultWhenModalIsValid()
        {
            var mockService = new Mock<ICosmosDatabaseService>();

            mockService.Setup(x => x.AddCustomerDetails(It.IsAny<CustomerDetails>())).Returns(Task.FromResult("MC183939"));

            var controller = new TokenController(mockService.Object);

            var customerDetails = new CustomerDetails
            {
                CustomerName = "Raj",
                CustomerAge = 29,
                CustomerSocialNumber = 166382,
                CustomerMobileNumber = 789928784,
                CustomerType = "Guest"
            };

            var actual = await controller.Post(customerDetails) as ObjectResult;

            Assert.AreEqual(StatusCodes.Status201Created, actual.StatusCode);
            Assert.IsNotNull(actual);
        }

        private static List<CustomerDashboard> getCustomerDaashboards()
        {
            return new List<CustomerDashboard>
            {
                new CustomerDashboard
                {
                    Counter = 1,
                    EstimatedWaitingTime = 5,
                    ServiceType = ServiceType.BankGeneralService,
                    TokenNumber = "MC20200905"
                },
                new CustomerDashboard
                {
                    Counter = 2,
                    EstimatedWaitingTime = 25,
                    ServiceType = ServiceType.BankTransactionService,
                    TokenNumber = "MC20200906"
                }
            };
        }
    }
}

