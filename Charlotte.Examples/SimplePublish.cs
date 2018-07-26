using System;
using System.Net.Mqtt;
using System.Threading.Tasks;

namespace Charlotte.Examples
{
    public class SimplePublish
    {
        public SimplePublish(string broker)
        {
            var mqtt = new Charlotte(broker);
            mqtt.On["sensors/bedroom/presence"] = async msg =>
            {
                if (msg.Message == "human present")
                {
                    await mqtt.Publish("lights/bedroom", "on");
                }
            };
        }
    }
}
