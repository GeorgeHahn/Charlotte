using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Charlotte
{
    public class MQTTConnection
    {
        private readonly MqttClient _client;
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;

        public event MqttClient.MqttMsgPublishEventHandler MqttMsgPublishReceived;

        public MQTTConnection(string brokerHostName, int brokerPort, string username, string password)
        {
            
            this._username = username;
            this._password = password;
            _clientId = "charl" + Guid.NewGuid().ToString().Substring(0, 6);

            _client = new MqttClient(brokerHostName, brokerPort, false, null);
            _client.MqttMsgPublishReceived += OnClientOnMqttMsgPublishReceived;
        }

        private void OnClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (MqttMsgPublishReceived != null)
                MqttMsgPublishReceived(sender, e);
        }

        public bool IsConnected
        {
            get { return _client.IsConnected; }
        }

        public void Connect()
        {
            try
            {
                if (!IsConnected)
                {
                    _client.Connect(_clientId, _username, _password);
                }
            }
            catch (MqttConnectionException e)
            {
                // LogTo.ErrorException("Couldn't connect to MQTT broker", e);
                throw;
            }
            catch (MqttCommunicationException e)
            {
                // Log
                throw;
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void Publish(string topic, string message)
        {
            if (!IsConnected)
                throw new Exception("Not connected");

            if (message == null)
                throw new ArgumentNullException("message");
            if (topic == null)
                throw new ArgumentNullException("topic");

            lock (_client)
            {
                _client.Publish(topic, Encoding.UTF8.GetBytes(message));
            }
        }

        internal void Subscribe(string[] topics, byte[] qosses)
        {
            _client.Subscribe(topics, qosses);
        }

        internal void Unsubscribe(string[] topics)
        {
            _client.Unsubscribe(topics);
        }

        public void Publish(string topic, string message, byte qos, bool retain)
        {
            if (!IsConnected)
                throw new Exception("Not connected");

            if (message == null)
                throw new ArgumentNullException("message");
            if (topic == null)
                throw new ArgumentNullException("topic");

            lock (_client)
            {
                _client.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
            }
        }
    }
}
