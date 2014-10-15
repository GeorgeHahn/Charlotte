namespace Charlotte.Examples
{
    public class SimplePublish : MqttModule
    {
        public SimplePublish()
            : base("localhost")
        {
            On["sensors/bedroom/presence"] = _ =>
            {
                if (_.Message == "human present")
                    Publish("lights/bedroom", "on");
            };
        }
    }
}
