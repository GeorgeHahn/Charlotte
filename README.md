# Charlotte [![Build status](https://ci.appveyor.com/api/projects/status/p54dccxh87xf4h5w?svg=true)](https://ci.appveyor.com/project/GeorgeHahn/charlotte)

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
