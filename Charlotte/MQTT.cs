using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Charlotte
{
    public class Mqtt
    {
        private readonly MqttClient _client;
        private readonly List<MqttHandler> _handlers;
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

            _handlers = new List<MqttHandler>();

            //LogTo.Debug("New MQTT created for {2}@{0}:{1}", brokerHostName, brokerPort, username);
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
                //LogTo.ErrorException("Couldn't connect to MQTT broker", e);
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

            lock (_client)
            {
                _client.Publish(topic, Encoding.UTF8.GetBytes(message));
            }
        }

        public void Publish(string topic, string message, byte qos, bool retain)
        {
            if (!IsConnected)
                throw new Exception("Not connected");

            lock (_client)
            {
                _client.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
            }
        }

        private void MqttMsgReceived(MqttMsgPublishEventArgs e)
        {
            var message = new MqttMessage { Message = Encoding.UTF8.GetString(e.Message), Topic = e.Topic };

            List<Action<MqttMessage>> actions = new List<Action<MqttMessage>>();

            lock (_handlers)
            {
                foreach (var handler in _handlers)
                {
                    if (topicMatcher.TopicsMatch(message, handler.Topic, e.Topic))
                        actions.Add(handler.Action);
                }
            }

            foreach (var action in actions)
                action(message);
        }

        public Action<dynamic> this[string topic]
        {
            set
            {
                topicMatcher.VerifyWildcardNames(topic);

                _client.Subscribe(new[] { topicMatcher.BoilWildcards(topic) }, new byte[] { 2 });
                lock (_handlers)
                {
                    _handlers.Add(new MqttHandler(topic, value));
                }
            }
        }
    }
}