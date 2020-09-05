namespace MicroCredential.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BankDashboard
    {
        public string id { get; set; }

        public string TokenNumber { get; set; }

        public string ServiceType { get; set; }

        public string StatusType { get; set; }

        public string CustomerType { get; set; }
    }
}
