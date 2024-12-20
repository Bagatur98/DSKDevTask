using DSKDevTask.Models;
using Microsoft.AspNetCore.Mvc;

namespace DSKDevTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditController : ControllerBase
    {
        
        private readonly ILogger<CreditController> _logger;
        private readonly CreditRepository _dskRepository;

        public CreditController(ILogger<CreditController> logger, CreditRepository dskRepository)
        {
            _logger = logger;
            _dskRepository = dskRepository;
        }

        [HttpGet]
        [Route("GetAllCredits")]
        public async Task<IEnumerable<Credit>> GetAllCredits()
        {
            return await _dskRepository.GetCreditsWithInvoices();
        }

        [HttpGet]
        [Route("GetPaidAndAwaitingTotals")]
        public async Task<PaidPercentViewModel> GetPaidAndAwaitingTotals()
        {
            return await _dskRepository.GetPaidAndAwaitingTotals();
        }
    }
}
