# Charlotte

## The Modern MQTT Framework

```csharp
public class ProximitySensor : MQTTModule
	: base("localhost", 1883)
{
	On["{room}/sensors/{sensor}"] = _ =>
	{
		Log("Data received from {0} (in {1}): {2}", _.sensor, _.room, _.Message);
	};

	On["{room}/sensors/presence"] = _ =>
	{
		if(_.Message == "human present")
			Publish(_.room + "/lights/", "on");
	}
}
```