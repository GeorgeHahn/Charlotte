using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anotar.NLog;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Charlotte
{
    public class Mqtt
    {
        private readonly MqttClient _client;
        private readonly Dictionary<string, Action<MqttMessage>> _handlers;
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;
        private readonly MqttTopicMatcher topicMatcher;

        public Mqtt(string brokerHostName, int brokerPort, string username, string password)
        {
            this._username = username;
            this._password = password;

            topicMatcher = new MqttTopicMatcher();

            _clientId = "charl" + Guid.NewGuid().ToString().Substring(0, 6);

            _client = new MqttClient(brokerHostName, brokerPort, false, null);
            _client.MqttMsgPublishReceived += (o, args) => MqttMsgReceived(args);

            _handlers = new Dictionary<string, Action<MqttMessage>>();

            LogTo.Debug("New MQTT created for {2}@{0}:{1}", brokerHostName, brokerPort, username);
        }


        public bool IsConnected => _client.IsConnected;

        public void Connect()
        {
            try
            {
                if(!IsConnected)
                    _client.Connect(_clientId, _username, _password);
            }
            catch(MqttConnectionException e)
            {
                LogTo.ErrorException("Couldn't connect to MQTT broker", e);
                throw;
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void Publish(string topic, string message)
        {
            lock (_client)
            {
                _client.Publish(topic, Encoding.UTF8.GetBytes(message));
            }
        }

        private void MqttMsgReceived(MqttMsgPublishEventArgs e)
        {
            var message = new MqttMessage { Message = Encoding.UTF8.GetString(e.Message), Topic = e.Topic };
            if (_handlers.ContainsKey(e.Topic))
                _handlers[e.Topic](message);
            
            foreach (string key in _handlers.Keys)
            {
                if (topicMatcher.TopicsMatch(message, key, e.Topic))
                    _handlers[key](message);
            }
        }

        public Action<dynamic> this[string topic]
        {
            set
            {
                _client.Subscribe(new[] { topicMatcher.BoilWildcards(topic) }, new byte[] { 2 });
                _handlers.Add(topic, value);
            }
        }
    }
}