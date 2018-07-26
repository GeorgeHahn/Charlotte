using System;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Charlotte;

namespace Charlotte
{
    public class Charlotte
    {
        public MqttRouter On { get; private set; }
        public IMqttClient Client { get; private set; }
        
        public Charlotte(string IP)
            :this(IP, new MqttClientCredentials(new Guid().ToString()), new MqttConfiguration())
        { }

        public Charlotte(string IP, MqttClientCredentials creds)
            :this(IP, creds, new MqttConfiguration())
        { }

        public Charlotte(string IP, MqttConfiguration config)
            :this(IP, new MqttClientCredentials(new Guid().ToString()), config)
        { }

        public Charlotte(string IP, MqttClientCredentials creds, MqttConfiguration config)
        {
            var client = MqttClient.CreateAsync(IP, config);
            client.RunSynchronously();
            Client = client.Result;
            On = new MqttRouter(Client);
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

        public Charlotte(string IP, IMqttClient client)
            :this(IP, new MqttClientCredentials(new Guid().ToString()), client)
        { }

        public Charlotte(string v, MqttClientCredentials creds, IMqttClient client)
        {
            this.Client = client;
            On = new MqttRouter(client);
        }
    }
}