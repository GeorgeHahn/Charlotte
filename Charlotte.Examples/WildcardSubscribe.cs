using System;

namespace Charlotte.Examples
{
    public class WildcardSubscribe
    {
        public WildcardSubscribe(string broker)
        {
            var mqtt = new Mqtt(broker);
            mqtt.On["{room}/sensors/{sensor}"] = msg =>
            {
                Console.WriteLine($"{msg.sensor} in {msg.room} measured {msg.Message}");
            };
        }
    }
}
