using System;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Charlotte;

namespace Charlotte
{
    public class Mqtt
    {
        public MqttRouter On { get; private set; }
        public IMqttClient Client { get; private set; }
        
        /// Create MQTT router and MQTT client
        public Mqtt(string IP, int port=1883, MqttClientCredentials creds= null)
            :this(IP, new MqttConfiguration{Port = port}, creds)
        { }

        /// Create MQTT router and MQTT client
        public Mqtt(string IP, MqttConfiguration config, MqttClientCredentials creds = null)
        {
            var client = MqttClient.CreateAsync(IP, config);
            client.Wait();
            Client = client.Result;

            Task<SessionState> conn;
            if(creds == null)
                conn = Client.ConnectAsync();
            else
                conn = Client.ConnectAsync(creds);
            conn.Wait();

            On = new MqttRouter(Client);
        }

        /// Create MQTT router from existing client. Client should be connected.
        public Mqtt(string IP, IMqttClient client)
            :this(IP, new MqttClientCredentials(new Guid().ToString()), client)
        { }

        public Mqtt(string v, MqttClientCredentials creds, IMqttClient client)
        {
            this.Client = client;
            On = new MqttRouter(client);
        }

        public async Task Publish(string topic, string message, MqttQualityOfService qos = MqttQualityOfService.AtLeastOnce)
        {
            await Publish(topic, Encoding.UTF8.GetBytes(message), qos);
        }

        public async Task Publish(string topic, byte[] message, MqttQualityOfService qos = MqttQualityOfService.AtLeastOnce)
        {
            var msg = new MqttApplicationMessage(topic, message);
            await Client.PublishAsync(msg, qos);
        }
    }
}