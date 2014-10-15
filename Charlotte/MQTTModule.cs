using System;
using System.Collections.Generic;

namespace Charlotte
{
    abstract public class MqttModule
    {
        protected Mqtt On;
        protected Action<string, string> Publish;
        protected Action OnConnect;

        private static readonly Dictionary<MqttHost, Mqtt> Clients = new Dictionary<MqttHost, Mqtt>();
        private static readonly Dictionary<MqttHost, int> ClientUseCount = new Dictionary<MqttHost, int>();
        private readonly MqttHost _thishost;

        public bool IsConnected => On.IsConnected;

        public void Run() => Connect();

        protected virtual void Stop() { }

        protected MqttModule(string brokerHostName, int brokerPort, string username, string password)
        {
            _thishost = new MqttHost(brokerHostName, brokerPort, username);
            if (!Clients.ContainsKey(_thishost))
            {
                Clients[_thishost] = new Mqtt(brokerHostName, brokerPort, username, password);
                ClientUseCount[_thishost] = 1;
            }
            else
            {
                ClientUseCount[_thishost]++;
            }

            On = Clients[_thishost];

            Publish = (topic, message) =>
            {
                On.Publish(topic, message);
            };
        }

        protected MqttModule(string brokerHostName, int brokerPort)
            : this(brokerHostName, brokerPort, "", "")
        { }

        protected MqttModule(string brokerHostName)
            : this(brokerHostName, 1883, "", "")
        { }

        protected void Connect()
        {
            On.Connect();
            if (OnConnect != null)
                OnConnect();
        }

        public void Disconnect()
        {
            Stop();
            ClientUseCount[_thishost]--;

            if(ClientUseCount[_thishost] <= 0)
                On.Disconnect();
        }
    }
}