namespace MicroCredentialUnitTestProject.ServiceTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MicroCredential.Models;
    using MicroCredential.Properties;
    using MicroCredential.Services;
    using Microsoft.Azure.Cosmos;
    using Moq;
    using NUnit.Framework;

    class CosmosDatabaseServiceTest
    {
        [Test]
        public async Task AddCustomerDetailsASExpected()
        {
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockQueyable = new Mock<IOrderedQueryable>();

            var customerDetails = new List<CustomerDetails>
            {
                new CustomerDetails{ CustomerName = "Rajprabhu", CustomerType = CustomerType.Guest}
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<CustomerDetails>(true, null, null)).Returns((IOrderedQueryable<CustomerDetails>)customerDetails);
            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var service = new CosmosDatabaseService(mockCosmosClient.Object, "Connectionstring", "Collection");

            var actual = await service.AddCustomerDetails(new CustomerDetails());

            Assert.NotNull(actual);
        }

        [Test]
        public async Task AsExpectedCustomerDetails()
        {
            var queryableCustomerDetails = new List<CustomerDetails>
            {
                new CustomerDetails{CustomerName = "raj", CustomerMobileNumber = 789472664, TokenNumber="MC16373828"}
            }.AsQueryable();

            Mock<CosmosClient> mockCosmosClient = MockCosmosClient(queryableCustomerDetails);

            var service = new CosmosDatabaseService(mockCosmosClient.Object, "Connectionstring", "Collection");

            var actual = await service.UpdateCustomerDetails("MC16373828", StatusType.Served);

            Assert.IsTrue(actual);
        }

        [Test]
        public void AsExpectedBankTokenDahboard()
        {
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockQueryable = new Mock<IOrderedQueryable>();

            var queryableResult = new List<BankDashboard>
            {
                new BankDashboard{ id="12345", TokenNumber="MC13939273", CustomerType = CustomerType.Guest, StatusType = StatusType.InCounter, ServiceType = ServiceType.BankGeneralService }
            }.AsQueryable();

            mockContainer.Setup(x => x.GetItemLinqQueryable<BankDashboard>(true, null, null)).Returns((IOrderedQueryable<BankDashboard>)queryableResult);
            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var service = new CosmosDatabaseService(mockCosmosClient.Object, "Connectionstring", "Collection");

            var actual = service.GetBankDashboards(ServiceType.BankGeneralService, StatusType.InCounter, CustomerType.Guest);

            Assert.AreEqual(actual.Count(), queryableResult.Count());
        }

        [Test]
        public void AsExpectedCustomerTokenDashboard()
        {
            var customerDetails = new List<CustomerDetails>
            {
                new CustomerDetails{CustomerAge = 29, CustomerName= "Raj"}
            }.AsQueryable();

            Mock<CosmosClient> mockCosmosClient = MockCosmosClient(customerDetails);

            var service = new CosmosDatabaseService(mockCosmosClient.Object, "Connectionstring", "Collection");

            var actual = service.GetCustomerDashboards();

            Assert.AreEqual(1, actual.Count());
        }

        private static Mock<CosmosClient> MockCosmosClient(IEnumerable<CustomerDetails> queryableCustomerDetails)
        {
            var mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockQueryable = new Mock<IOrderedQueryable>();

            mockContainer.Setup(x => x.GetItemLinqQueryable<CustomerDetails>(true, null, null)).Returns((IOrderedQueryable<CustomerDetails>)queryableCustomerDetails);
            mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);
            return mockCosmosClient;
        }
    }
}
