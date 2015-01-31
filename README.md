# Charlotte [![AppVeyor branch](https://img.shields.io/appveyor/ci/GeorgeHahn/Charlotte/master.svg)](https://ci.appveyor.com/project/GeorgeHahn/charlotte) [![NuGet](https://img.shields.io/nuget/v/Charlotte.svg)](https://www.nuget.org/packages/Charlotte)

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
