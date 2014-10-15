using System;

namespace Charlotte.Examples
{
    public class WildcardSubscribe : MqttModule
    {
        public WildcardSubscribe()
            : base("localhost")
        {
            On["{room}/sensors/{sensor}"] = _ =>
            {
                Console.WriteLine("{0} in {1} measured {2}", _.room, _.sensor, _.Message);
            };
        }
    }
}
