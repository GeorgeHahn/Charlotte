using System;
using System.Collections.Generic;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace Charlotte
{
    public class MqttRouter
    {
        private readonly List<MqttHandler> _handlers;
        private readonly MqttTopicMatcher _topicMatcher;
        private readonly IMqttClient _connection;

        public MqttRouter(IMqttClient connection)
        {
            _connection = connection;
            _topicMatcher = new MqttTopicMatcher();
            _handlers = new List<MqttHandler>();

            connection
                .MessageStream
                .Subscribe(msg => MqttMsgReceived(msg));
        }

        private void MqttMsgReceived(MqttApplicationMessage msg)
        {
            var message = new MqttMessage { Message = msg.Payload == null ? null : Encoding.UTF8.GetString(msg.Payload), Topic = msg.Topic };

            List<Action<MqttMessage>> actions = new List<Action<MqttMessage>>();

            lock (_handlers)
            {
                foreach (var handler in _handlers)
                {
                    if (_topicMatcher.TopicsMatch(message, handler.Topic, msg.Topic))
                    {
                        actions.Add(handler.Action);
                    }
                }
            }

            foreach (var action in actions)
            {
                action(message);
            }
        }

        private List<string> subscriptions = new List<string>();
        public Action<dynamic> this[string topic]
        {
            set
            {
                var sub = _topicMatcher.ConvertMatchingGroupsToMQTTWildcards(topic);
                subscriptions.Add(sub);

                if(!_connection.IsConnected)
                {
                    throw new MqttException("Cannot subscribe when client is disconnected");
                }

                // TODO: Subscribe QoS customization
                _connection.SubscribeAsync(sub, MqttQualityOfService.AtLeastOnce);
                lock (_handlers)
                {
                    _handlers.Add(new MqttHandler(topic, value));
                }
            }
        }
    }
}