using System;

namespace Charlotte.Examples
{
    public class SimpleSubscribe
    {
        public SimpleSubscribe(string broker)
        {
            var mqtt = new Charlotte(broker);
            mqtt.On["lights/bedroom"] = msg =>
            {
                Console.WriteLine("Bedroom lights set to {0}", msg.Message);
            };
        }
    }
}
