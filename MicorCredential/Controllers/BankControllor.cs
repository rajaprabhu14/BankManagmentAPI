namespace MicroCredential.Controllers
{
    using MicroCredential.Models;
    using MicroCredential.Properties;
    using MicroCredential.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/Bank")]
    [ApiController]
    public class BankControllor : ControllerBase
    {
        private readonly ICosmosDatabaseService cosmosDatabaseService;

        public BankControllor(ICosmosDatabaseService cosmosDatabaseService)
        {
            this.cosmosDatabaseService = cosmosDatabaseService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<BankDashboard> Get(string servicetype, string statusType, string cutsomerType)
        {
            return this.cosmosDatabaseService.GetBankDashboards(servicetype, statusType, cutsomerType);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(string tokenNumber, string status)
        {
            if(!string.IsNullOrEmpty(status) && (status == StatusType.InCounter || status == StatusType.Served))
            {
                var result = await this.cosmosDatabaseService.UpdateCustomerDetails(tokenNumber, status);
                return (ActionResult)(result ? Ok():(IActionResult)NotFound());
            }

            return BadRequest("Please check the statusType");
        }

    }
}
