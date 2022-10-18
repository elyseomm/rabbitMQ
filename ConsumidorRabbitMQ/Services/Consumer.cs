using ConsumidorRabbitMQ.Database;
using ConsumidorRabbitMQ.Pages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsumidorRabbitMQ.Services
{
    public class Consumer
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RabbitMQConfig _config;
        private readonly SQLiteConfig _sqLiteConfig;


        public Consumer(RabbitMQConfig config, SQLiteConfig sqLiteConfig, ILogger<IndexModel> logger)
        {
            _config = config;
            _logger = logger;
            _sqLiteConfig = sqLiteConfig;

            // * Cria/Verifica database creation
            new DBContext(_sqLiteConfig);
        }

        public void Start()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.Host,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
                //ContinuationTimeout = new TimeSpan(0, 1, 0, 0)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {                
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {                    
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);


                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        var jObj = JObject.Parse(message);

                        if (jObj.ContainsKey("filename"))
                        {
                            //{MachineName}_{ProgramName}_{DataHora}.txt"

                            var dados = jObj.Value<string>("filename");

                            var infos = dados.Replace(".txt", "").Split('_');

                            var programName = infos[1].Trim();

                            // * Verificar lista de grupos de nomes de programas configurados
                            if (GrupoProgramasAdapter.CheckIsGrupo(programName))
                            {
                                programName = GrupoProgramasAdapter.GrupoPrograma;
                                ProgramaAdapter.UpdateCounter(programName);
                            }
                            else
                            {
                                programName = infos[1];
                                ProgramaAdapter.UpdateCounter(programName);
                            }

                            //* SAÍDA => -log: "count = {valor} for file {nomeAplicacao} {dateTime}"
                            var log = $"count = {ProgramaAdapter.Contador} for file {programName} {infos[2]}";
                            _logger.LogInformation(log, null);
                        }
                    }

                };
                channel.BasicConsume(queue: _config.QueueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
        }
    }
}
