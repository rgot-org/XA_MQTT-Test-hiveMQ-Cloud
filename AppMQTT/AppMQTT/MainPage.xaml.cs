using MQTTnet;
using MQTTnet.Client;

using System;
using System.IO;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppMQTT
{
    public partial class MainPage : ContentPage
    {
        MqttHandler client = new MqttHandler("config.json");        
        public MainPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MqttHandler,String>(this, "message", (sender, message) => _majProprietes(message));
            MessagingCenter.Subscribe<MqttHandler>(this, "connected", (sender) => _majConnexion(sender));
        }

        private void _majProprietes(String message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Label_MessageRecu.Text = message;
            });
        }
        private void _majConnexion(MqttHandler sender)
        {
            if (sender.isConnected())
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    status.Text = "connecté";
                });
            }            
            
        }



        /*******************************************************************************/
        /*                     gestion des événements de l'IHM                         */
        /*******************************************************************************/
        private void btn_Publier_Clicked(object sender, EventArgs e)
        {
            client.publishMessage(Entry_MessageToSend.Text);
        }

        
    }
}
