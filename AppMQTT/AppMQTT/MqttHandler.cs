using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppMQTT
{
    class MqttHandler
    {
        MqttFactory factory = new MqttFactory();
        private IMqttClient mqttClient;
        private Config _config { get; set; }
        public MqttHandler( string config)
        {
            _config = Config.init(config);
            mqttClient = factory.CreateMqttClient();
            mqttClient.ConnectedAsync += (e => ConnectedHandler(e));// une fois que le client est connecte appelle la fonction  "ConnectedHandler"
            mqttClient.ApplicationMessageReceivedAsync += e => MessageReceivedHandler(e);// à la reception d'un message appelle la fonction "MessageReceivedHandler"
            mqttClient.DisconnectedAsync += e => DisconnectedHandler(e);// à la deconnexion apelle "DisconnectedHandler"
            Task.Run(async () => { await Connect(); }); // connexion initiale
        }
        public Boolean isConnected()
        {
            return mqttClient.IsConnected;
        }
        public async Task Connect()
        {
            // voir https://github.com/chkr1011/MQTTnet/wiki/Client#tcp-connection 
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.Server, _config.Port)
                .WithClientId(Guid.NewGuid().ToString())
                .WithTls(new MqttClientOptionsBuilderTlsParameters() //https://github.com/chkr1011/MQTTnet/issues/1211
                {
                    UseTls = true, // Is set by default to true, I guess...
                    SslProtocol = SslProtocols.Tls12, // TLS downgrade
                    AllowUntrustedCertificates = true, // Not sure if this is really needed...
                    IgnoreCertificateChainErrors = true, // Not sure if this is really needed...
                    IgnoreCertificateRevocationErrors = true, // Not sure if this is really needed...
                    CertificateValidationHandler = (w) => true // Not sure if this is really needed...
                })
                .WithCredentials(_config.User, _config.Password)
                .WithCleanSession()
            .Build();
            try // essaye
            {
                // voir https://github.com/chkr1011/MQTTnet/wiki/Client#connecting
                await mqttClient.ConnectAsync(options, CancellationToken.None);
            }
            catch (Exception ex) // en cas d'echec
            {
                Console.WriteLine(ex);
            }
        }

        public async void publishMessage(String value)
        {
            // voir https://github.com/chkr1011/MQTTnet/wiki/Client#publishing-messages
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_config.PubTopic)
                .WithPayload(value)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();
            await mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }
        /*******************************************************************************/
        /*                   zone des handler (gestionnaires)                          */
        /*******************************************************************************/
        private async Task ConnectedHandler(MqttClientConnectedEventArgs arg)
        {
             MessagingCenter.Send(this, "connected");
            // Subscribe to a topic
            // voir https://github.com/chkr1011/MQTTnet/wiki/Client#subscribing-to-a-topic
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_config.SubTopic).Build());
        }
        private Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs arg)
        {
            // voir https://github.com/chkr1011/MQTTnet/wiki/Client#consuming-messages
            //recupération le la payload et conversion byte[]-> string
            String str = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            MessagingCenter.Send(this,"message", str);
            return Task.CompletedTask;
        }


        // à la deconnexion
        private async Task DisconnectedHandler(MqttClientDisconnectedEventArgs arg)
        {
            // voir https://github.com/chkr1011/MQTTnet/wiki/Client#reconnecting
            await Task.Delay(TimeSpan.FromSeconds(5));
            await Connect();
        }
    }
}
