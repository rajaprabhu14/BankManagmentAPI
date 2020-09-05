namespace MicroCredentialUnitTestProject.ControllerTest
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MicroCredential.Controllers;
    using MicroCredential.Models;
    using MicroCredential.Properties;
    using Moq;
    using MicroCredential.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using System.Linq;


    public class BankControllerTest
    {
        [Test]
        public void AsExpectedBankDashboard()
        {
            var mockService = new Mock<ICosmosDatabaseService>();
            var controller = new BankControllor(mockService.Object);

            List<BankDashboard> bankDashboards = getBankDashboards();

            mockService.Setup(x => x.GetBankDashboards(ServiceType.BankGeneralService, StatusType.InCounter, CustomerType.Guest)).Returns(bankDashboards);

            var actual = controller.Get(ServiceType.BankGeneralService, StatusType.InCounter, CustomerType.Guest);

            Assert.NotNull(actual);
            Assert.AreEqual(bankDashboards.Count, actual.Count());
        }

        [Test]
        [TestCase("Test")]
        public async Task AsExpectedResultWhenModalIsInvalid(String status)
        {
            var mockService = new Mock<ICosmosDatabaseService>();

            var controller = new BankControllor(mockService.Object);

            var actual = await controller.Put("MC163868", status) as BadRequestObjectResult;

            Assert.AreEqual(StatusCodes.Status400BadRequest, actual.StatusCode);
            Assert.AreEqual("Please check the statusType", actual.Value);
        }

        [Test]
        public async Task AsExpectedResultWhenModalIsValid()
        {
            var mockService = new Mock<ICosmosDatabaseService>();

            mockService.Setup(x => x.UpdateCustomerDetails(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var controller = new BankControllor(mockService.Object);

            var actual = await controller.Put("MC121414", StatusType.Served) as OkResult;

            Assert.AreEqual(StatusCodes.Status200OK, actual.StatusCode);
        }

        private static List<BankDashboard> getBankDashboards()
        {
            return new List<BankDashboard>
            {
                new BankDashboard
                {
                    id = Guid.NewGuid().ToString(),
                    CustomerType = CustomerType.Guest,
                    StatusType = StatusType.InCounter,
                    TokenNumber = "MC20200905",
                    ServiceType = ServiceType.BankGeneralService
                },
                new BankDashboard
                {
                    id = Guid.NewGuid().ToString(),
                    CustomerType = CustomerType.BankCustomer,
                    StatusType = StatusType.InCounter,
                    TokenNumber = "MC20200905",
                    ServiceType = ServiceType.BankTransactionService
                }
            };
        }
    }
}
