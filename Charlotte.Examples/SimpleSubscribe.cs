using System;

namespace Charlotte.Examples
{
    public class SimpleSubscribe : MqttModule
    {
        public SimpleSubscribe()
            : base("localhost")
        {
            On["lights/bedroom"] = _ =>
            {
                Console.WriteLine("Bedroom lights set to {0}", _.Message);
            };
        }
    }
}
