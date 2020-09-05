namespace MicroCredential.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MicroCredentialSetting
    {
        public string EndpointUrl { get; set; }
        public string PrimaryKey { get; set; }

        public string DatabaseName { get; set; }

        public string ContainerName { get; set; }
    }
}
