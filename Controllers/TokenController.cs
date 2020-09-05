namespace MicroCredential.Controllers
{
    using MicroCredential.Models;
    using MicroCredential.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    [DisplayName("CustomerToken")]
    public class TokenController : ControllerBase
    {

        private readonly ICosmosDatabaseService cosmosDatabaseService;
        public TokenController(ICosmosDatabaseService cosmosDatabaseService)
        {
            this.cosmosDatabaseService = cosmosDatabaseService;
        }

        [HttpGet]
        [SwaggerOperation(Tags = new [] {"CustomerDashboard"})]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<CustomerDashboard> Get()
        {
            return this.cosmosDatabaseService.GetCustomerDashboards();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerDetails customerDetails)
        {
            if (ModelState.IsValid)
            {
                var result = await this.cosmosDatabaseService.AddCustomerDetails(customerDetails);
                return StatusCode(StatusCodes.Status201Created, result);
            }

            return BadRequest("Please check input details");
        }
    }
}
