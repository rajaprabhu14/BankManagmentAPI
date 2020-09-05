namespace MicroCredential.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class CustomerDetails
    {
        [Key]
        [JsonProperty(propertyName: "id")]

        public string Id { get; set; }

        [JsonProperty(propertyName: "customerName")]
        [Required]
        public string CustomerName { get; set; }

        [JsonProperty(propertyName: "customerAge")]
        [Required]
        public int CustomerAge { get; set; }

        [JsonProperty(propertyName: "customerMobileNumber")]
        [Required]
        [Display]
        public int CustomerMobileNumber { get; set; }

        [JsonProperty(propertyName: "customerType")]
        [Required]
        [RegularExpression(Properties.CustomerType.BankCustomer + "|" + Properties.CustomerType.Guest)]
        public string CustomerType { get; set; }

        [JsonProperty(propertyName: "customerAccountNumber")]
        public int CustomerAccountNumber { get; set; }

        [JsonProperty(propertyName: "customerSocialNumber")]
        [Required]
        public int CustomerSocialNumber { get; set; }

        [Required]
        [RegularExpression(Properties.ServiceType.BankTransactionService + "|" + Properties.ServiceType.BankGeneralService)]
        public string ServiceType { get; set; }

        public string Status { get; set; }

        public string TokenNumber { get; set; }

        public int Counter { get; set; }
    }
}
