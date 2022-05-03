using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SystemGlobalServices_TestTask.Models;

namespace SystemGlobalServices_TestTask.Controllers
{
    [ApiController]
    [Route("CurrenciesApi")]
    public class CurrenciesController : ControllerBase
    {
        string currenciesFileUrl = "https://www.cbr-xml-daily.ru/daily_json.js";

        public async Task<Dictionary<string, Currency>> getValutes()
        {
            using HttpClient httpClient = new HttpClient();
            string jsonString = await httpClient.GetStringAsync(currenciesFileUrl);
            string valuteString = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString)["Valute"].ToString();
            var currencyDictionary = JsonSerializer.Deserialize<Dictionary<string, Currency>>(valuteString);
            return currencyDictionary;
        }

        // GET /currencies — возвращает список курсов валют
        [HttpGet("currencies")]
        [Produces("application/json")]
        public async Task<ActionResult> Get()
        {
            Dictionary<string, Currency> currencyDictionary = null;
            List<Currency> currencyList = null;
            try
            {
                currencyDictionary = getValutes().Result;
                currencyList = currencyDictionary.Values.ToList();
            }
            catch
            {
                return BadRequest();
            }
            if (currencyList == null)
                return NotFound();
            return new ObjectResult(currencyList);
        }

        // GET /currency/ — возвращает курс валюты для переданного идентификатора валюты
        [HttpGet("currency/{CharCode}")]
        [Produces("application/json")]
        public async Task<ActionResult> Get(string CharCode)
        {
            Dictionary<string, Currency> currencyDictionary = null;
            Currency currency = null;
            try
            {
                currencyDictionary = getValutes().Result;
            }
            catch
            {
                return BadRequest();
            }
            currency = currencyDictionary[CharCode];
            if (currency == null)
                return NotFound();
            return new ObjectResult(currency);
        }
    }
}
