using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyMicroService.Controllers
{
    [ApiController]
    [Route("api/ETSCurrencies")]
    public class ETSCurrencyController : ControllerBase
    {
        private readonly IETSCurrencyService _currencyService;

        public ETSCurrencyController(IETSCurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ETSCurrencyResult>>> GetLatestETSCurrencies()
        {
            var currencies = await _currencyService.GetLatestETSCurrenciesAsync();

            if (currencies == null || !currencies.Any())
            {
                return NotFound("No data available");
            }

            return Ok(currencies);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ETSCurrencyResult>> GetETSCurrencyByCode([FromRoute] string code)
        {
            var currency = await _currencyService.GetLatestETSCurrencyByCodeAsync(code.ToUpper());

            if (currency == null)
            {
                return NotFound($"No data available for code {code}");
            }

            return Ok(currency);
        }
    }
}
