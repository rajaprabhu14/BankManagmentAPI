namespace MicroCredential.Services
{
    using MicroCredential.Models;
    using MicroCredential.Properties;
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CosmosDatabaseService : ICosmosDatabaseService
    {
        private Container container;

        public CosmosDatabaseService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            this.container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<string> AddCustomerDetails(CustomerDetails customerDetails)
        {
            if (customerDetails.CustomerType == CustomerType.Guest)
            {
                customerDetails.Id = Guid.NewGuid().ToString();
                customerDetails.TokenNumber = "MC" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Millisecond;
                customerDetails.ServiceType = ServiceType.BankGeneralService;
                customerDetails.Status = StatusType.Inqueue;
                customerDetails.CustomerSocialNumber = customerDetails.CustomerSocialNumber;
            }
            else
            {
                customerDetails.Id = Guid.NewGuid().ToString();
                customerDetails.TokenNumber = "MC" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Millisecond;
                customerDetails.Status = StatusType.Inqueue;
                customerDetails.CustomerAccountNumber = DateTime.Now.Year + DateTime.Now.Month + +DateTime.Now.Millisecond;
            }


            await this.container.CreateItemAsync<CustomerDetails>(customerDetails, new PartitionKey(customerDetails.ServiceType));

            return customerDetails.TokenNumber;
        }

        public async Task<bool> UpdateCustomerDetails(string tokenNumber, string status)
        {
            var customer = this.container.GetItemLinqQueryable<CustomerDetails>(true).Where(x => x.TokenNumber == tokenNumber).AsEnumerable().FirstOrDefault();
            if (customer != null)
            {
                customer.Counter = customer.ServiceType == ServiceType.BankTransactionService ? 1 : 2;
                customer.Status = status;
                if (customer.CustomerType == CustomerType.Guest)
                {
                    customer.CustomerAccountNumber = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Millisecond;
                }
                await this.container.UpsertItemAsync<CustomerDetails>(customer, new PartitionKey(customer.ServiceType));
                return true;
            }
            return false;
        }

        public IList<BankDashboard> GetBankDashboards(string servicetype, string statusType, string cutsomerType)
        {
            return this.container.GetItemLinqQueryable<BankDashboard>(true).AsQueryable().Where(x => x.StatusType == statusType || x.ServiceType == servicetype || x.CustomerType == cutsomerType).ToList();
        }

        public IList<CustomerDashboard> GetCustomerDashboards()
        {
            var customerDetailsList = this.container.GetItemLinqQueryable<CustomerDetails>(true).Where(customerDetail => customerDetail.Status != StatusType.Served).AsQueryable().ToList();

            int bankTransactionServiceQueue = 0;
            int bankGeneralServiceQueue = 0;

            List<CustomerDashboard> customerDashboards = new List<CustomerDashboard>();

            foreach (var items in customerDetailsList)
            {
                var customerTokenDashboards = new CustomerDashboard
                {
                    Counter = items.Counter,
                    TokenNumber = items.TokenNumber,
                    ServiceType = items.ServiceType
                };

                if (items.Status == StatusType.Inqueue)
                {
                    if (items.ServiceType == ServiceType.BankTransactionService)
                    {
                        ++bankTransactionServiceQueue;
                        customerTokenDashboards.EstimatedWaitingTime = 5 * bankTransactionServiceQueue;

                    }

                    if (items.ServiceType == ServiceType.BankGeneralService)
                    {
                        ++bankGeneralServiceQueue;
                        customerTokenDashboards.EstimatedWaitingTime = 25 * bankGeneralServiceQueue;
                    }
                }

                customerDashboards.Add(customerTokenDashboards);
            }

            return customerDashboards;
        }
    }
}
