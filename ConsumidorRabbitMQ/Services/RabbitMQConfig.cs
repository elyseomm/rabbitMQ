﻿using System.Collections.Generic;

namespace ConsumidorRabbitMQ.Services
{
    public class RabbitMQConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
