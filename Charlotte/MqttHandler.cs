using System;

namespace Charlotte
{
    public class MqttHandler
    {
        public string Topic { get; private set; }
        public Action<MqttMessage> Action { get; private set; }

        public MqttHandler(string topic, Action<MqttMessage> action)
        {
            Topic = topic;
            Action = action;
        }
    }
}