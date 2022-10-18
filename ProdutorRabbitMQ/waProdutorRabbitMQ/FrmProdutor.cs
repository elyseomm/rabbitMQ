using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace waProdutorRabbitMQ
{
    public partial class FrmProdutor : Form
    {
        private static RabbitMQConfig config { get; set; }        

        public FrmProdutor()
        {
            InitializeComponent();
            _ = int.TryParse(ConfigurationManager.AppSettings.Get("rabbitmq_port"), out int port);

            config = new RabbitMQConfig()
            {
                Host = ConfigurationManager.AppSettings.Get("rabbitmq_host").ToString(),
                Port = port,
                UserName = ConfigurationManager.AppSettings.Get("rabbitmq_user").ToString(),
                Password = ConfigurationManager.AppSettings.Get("rabbitmq_password").ToString(),
                QueueName = ConfigurationManager.AppSettings.Get("rabbitmq_queuename").ToString(),
            };

            Produtor.RabbitConfig = config;
        }

        private void FrmProdutor_Load(object sender, EventArgs e)
        {
            // * Atribuindo as configurações carregadas aos componentes de tela
            txtServer.Text = config.Host;
            txtPort.Text = config.Port.ToString();
            txtQueueName.Text = config.QueueName;

        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            txtMsg.Text = Produtor.GenerateJsonBody();
            btnEnviar.Enabled = txtMsg.Text.Trim().Length > 0;
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            Produtor.Send();
            txtMsg.Text = $"{txtMsg.Text}(Enviado)";
            btnEnviar.Enabled = !txtMsg.Text.Contains("(Enviado)");
        }
    }
}
