using System;

namespace Charlotte.Examples
{
    public class WildcardSubscribe
    {
        public WildcardSubscribe(string broker)
        {
            var mqtt = new Charlotte(broker);
            mqtt.On["{room}/sensors/{sensor}"] = _ =>
            {
                Console.WriteLine("{0} in {1} measured {2}", _.room, _.sensor, _.Message);
            };
        }
    }
}
