using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public StockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(Name = "stock={stockCode}")]
        public async Task Get(string stockCode)
        {
            var url = _configuration.GetValue<string>("ChatUrl");
            await Helper.CsvHelper.PostStockQuoteInformationToAllChatRooms(url, stockCode);
        }
    }
}
