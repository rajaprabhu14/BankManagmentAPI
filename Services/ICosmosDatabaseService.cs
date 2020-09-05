namespace MicroCredential.Services
{
    using System.Collections.Generic;
    using MicroCredential.Models;
    using System.Threading.Tasks;

    public interface ICosmosDatabaseService
    {
        IList<CustomerDashboard> GetCustomerDashboards();

        IList<BankDashboard> GetBankDashboards(string servicetype, string statusType, string cutsomerType);

        Task<string> AddCustomerDetails(CustomerDetails customerDetails);

        Task<bool> UpdateCustomerDetails(string id, string status);
    }
}
