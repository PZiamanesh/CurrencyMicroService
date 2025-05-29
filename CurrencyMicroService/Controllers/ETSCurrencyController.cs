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
                return NotFound();
            }

            return Ok(currencies);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ETSCurrencyResult>> GetETSCurrencyByCode([FromRoute] string code)
        {
            var currency = await _currencyService.GetLatestETSCurrencyByCodeAsync(code.ToUpper());

            if (currency == null)
            {
                return NotFound($"Currency {code.ToUpper()} not found");
            }

            return Ok(currency);
        }

        [HttpGet("by-date/{date:datetime}")]
        public async Task<ActionResult<IEnumerable<ETSCurrencyResult>>> GetETSCurrenciesByDate([FromRoute] DateTime date)
        {
            var currencies = await _currencyService.GetETSCurrenciesByDateAsync(date);

            if (currencies == null || !currencies.Any())
            {
                return NotFound($"No currencies found for date {date:yyyy-MM-dd}");
            }

            return Ok(currencies);
        }

        [HttpGet("{code}/by-date/{date:datetime}")]
        public async Task<ActionResult<ETSCurrencyResult>> GetETSCurrencyByCodeAndDate(
            [FromRoute] string code,
            [FromRoute] DateTime date)
        {
            var currency = await _currencyService.GetETSCurrencyByCodeAndDateAsync(code.ToUpper(), date);

            if (currency == null)
            {
                return NotFound($"Currency {code.ToUpper()} not found for date {date:yyyy-MM-dd}");
            }

            return Ok(currency);
        }
    }
}
