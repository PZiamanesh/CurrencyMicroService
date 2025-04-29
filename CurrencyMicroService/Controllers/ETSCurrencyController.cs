using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                throw new NotFoundException("No ETS currency data available");
            }

            return Ok(currencies);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ETSCurrencyResult>> GetETSCurrencyByCode([FromRoute] string code)
        {
            var currency = await _currencyService.GetLatestETSCurrencyByCodeAsync(code.ToUpper());

            if (currency == null)
            {
                throw new NotFoundException($"No ETS currency data available for code {code}");
            }

            return Ok(currency);
        }
    }
}
