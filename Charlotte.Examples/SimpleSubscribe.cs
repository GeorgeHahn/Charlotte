using System;

namespace Charlotte.Examples
{
    public class SimpleSubscribe
    {
        public SimpleSubscribe(string broker)
        {
            var mqtt = new Mqtt(broker);
            mqtt.On["lights/bedroom"] = msg =>
            {
                Console.WriteLine("Bedroom lights set to {0}", msg.Message);
            };
        }
    }
}
