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
        private readonly List<MqttHandler> _handlers;
        private readonly MqttTopicMatcher _topicMatcher;
        private readonly MQTTConnection _connection;

        public Mqtt(MQTTConnection connection)
        {
            _connection = connection;
            _topicMatcher = new MqttTopicMatcher();
            _handlers = new List<MqttHandler>();

            _connection.MqttMsgPublishReceived += OnConnectionOnMqttMsgPublishReceived;
        }

        private void OnConnectionOnMqttMsgPublishReceived(object o, MqttMsgPublishEventArgs args)
        {
            MqttMsgReceived(args);
        }

        internal void Disconnect()
        {
            foreach (var handler in _handlers)
            {
                _connection.Unsubscribe(new[] {_topicMatcher.BoilWildcards(handler.Topic) });
            }

            _connection.MqttMsgPublishReceived -= OnConnectionOnMqttMsgPublishReceived;
        }

        private void MqttMsgReceived(MqttMsgPublishEventArgs e)
        {
            var message = new MqttMessage { Message = Encoding.UTF8.GetString(e.Message), Topic = e.Topic };

            List<Action<MqttMessage>> actions = new List<Action<MqttMessage>>();

            lock (_handlers)
            {
                foreach (var handler in _handlers)
                {
                    if (_topicMatcher.TopicsMatch(message, handler.Topic, e.Topic))
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
                _topicMatcher.VerifyWildcardNames(topic);

                _connection.Subscribe(new[] { _topicMatcher.BoilWildcards(topic) }, new byte[] { 2 });
                lock (_handlers)
                {
                    _handlers.Add(new MqttHandler(topic, value));
                }
            }
        }

        public void Publish(string topic, string message)
        {
            _connection.Publish(topic, message);
        }

        public void Publish(string topic, string message, bool retain)
        {
            _connection.Publish(topic, message, 2, retain);
        }
    }
}