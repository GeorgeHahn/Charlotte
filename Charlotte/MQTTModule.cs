using System;
using System.Collections.Generic;

namespace Charlotte
{
    abstract public class MqttModule
    {
        protected Mqtt On;
        //protected Action<string, string> Publish;
        protected Action OnConnect;

        private static readonly Dictionary<MqttHost, MQTTConnection> Clients = new Dictionary<MqttHost, MQTTConnection>();
        private static readonly Dictionary<MqttHost, int> ClientUseCount = new Dictionary<MqttHost, int>();
        private readonly MqttHost _thishost;

        public bool IsConnected
        {
            get { return Clients[_thishost].IsConnected; }
        }

        public void Run()
        {
            Connect();
        }

        protected virtual void OnStop() { }

        protected MqttModule(string brokerHostName, int brokerPort, string username, string password)
        {
            _thishost = new MqttHost(brokerHostName, brokerPort, username);
            if (!Clients.ContainsKey(_thishost))
            {
                Clients[_thishost] = new MQTTConnection(brokerHostName, brokerPort, username, password);
                ClientUseCount[_thishost] = 1;
            }
            else
            {
                ClientUseCount[_thishost]++;
            }

            On = new Mqtt(Clients[_thishost]);

            //Publish = (topic, message) =>
            //{
            //    On.Publish(topic, message);
            //};
        }

        protected MqttModule(string brokerHostName, int brokerPort)
            : this(brokerHostName, brokerPort, "", "")
        { }

        protected MqttModule(string brokerHostName)
            : this(brokerHostName, 1883, "", "")
        { }

        public void Publish(string topic, string message)
        {
            On.Publish(topic, message);
        }

        public void Publish(string topic, string message, bool retain)
        {
            On.Publish(topic, message, retain);
        }

        protected void Connect()
        {
            Clients[_thishost].Connect();
            if (OnConnect != null)
                OnConnect();
        }

        public void Disconnect()
        {
            OnStop();
            On.Disconnect();
            ClientUseCount[_thishost]--;

            if (ClientUseCount[_thishost] <= 0)
                Clients[_thishost].Disconnect();
        }
    }
}