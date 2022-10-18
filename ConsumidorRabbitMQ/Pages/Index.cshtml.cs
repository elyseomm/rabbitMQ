using ConsumidorRabbitMQ.Database;
using ConsumidorRabbitMQ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumidorRabbitMQ.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IOptions<RabbitMQConfig> _options;
        private readonly IOptions<SQLiteConfig> _sqllite;

        public IndexModel(ILogger<IndexModel> logger, 
            IOptions<RabbitMQConfig> options,
            IOptions<SQLiteConfig> sqlite
            )
        {
            _logger = logger;
            _options = options;
            _sqllite = sqlite;
        }

        public void OnGet()
        {   
            var _consumer = new Consumer(_options.Value, _sqllite.Value, _logger);
            _consumer.Start();

            ViewData.Add("Host", _options.Value.Host);
            ViewData.Add("Port", _options.Value.Port);

        }
    }
}
