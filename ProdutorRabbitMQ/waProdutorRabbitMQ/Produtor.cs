using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace waProdutorRabbitMQ
{
    public class Produtor
    {
        public static RabbitMQConfig RabbitConfig { get; set; }
        public static string MachineName { get; set; }
        public static string ProgramName { get; set; }
        public static string DataHora { get; set; }

        private static List<string> Maquinas { get; set; } = new List<string> { "Zeus", "Hera", 
            "Poseidon", "Atena", "Ares", "Demeter", "Apolo", "Artemis", "Hefesto", 
            "Afrodite", "Hermes" , "Dionísio" };

        private static List<string> Aplicativos { get; set; } = new List<string> { "RecargaIVR ", 
            "RecargaSMS", "Reembolso01", "Reembolso02", "Ticket01", "Ticket02", "Ticket03", 
            "ValeGAS", "ValeREF", "Pgto01", "PagamentoOutros" , "Bonus" };

        public static string GenerateJsonBody()
        {
            //{ NomeMaquina}{ NomeAplicacao}{ ano_mes_dia_hora_minuto_segundo}.txt
            //ex: ABCD_SMSSender_2021101131559.txt

            MachineName = Maquinas[new Random().Next(0, 11)].ToString();
            ProgramName = Aplicativos[new Random().Next(0, 11)].ToString();
            DataHora = DateTime.Now.ToString("yyyyMMddHHmmss");

            return GetBody();
        }

        public static string GetBody() =>$"{MachineName}_{ProgramName}_{DataHora}.txt";


        public static void Send()
        {
            var factory = new ConnectionFactory()
            {
                HostName = RabbitConfig.Host,
                Port = RabbitConfig.Port,
                UserName = RabbitConfig.UserName,
                Password = RabbitConfig.Password,
                //ContinuationTimeout = new TimeSpan(0, 1, 0, 0)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: RabbitConfig.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                var jsonBody = new JObject();
                jsonBody.Add("filename", GetBody());

                var body = Encoding.UTF8.GetBytes(jsonBody.ToString());

                channel.BasicPublish(exchange: "",
                                     routingKey: RabbitConfig.QueueName,
                                     basicProperties: null,
                                     body: body);
            }
        }

    }
}
