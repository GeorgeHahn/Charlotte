# Charlotte [![pipeline status](https://gitlab.com/GeorgeHahn/Charlotte/badges/master/pipeline.svg)](https://gitlab.com/GeorgeHahn/Charlotte/commits/master) [![NuGet](https://img.shields.io/nuget/v/Charlotte.svg)](https://www.nuget.org/packages/Charlotte)

## The Modern MQTT Framework

```csharp
public class ProximitySensor
{
	public ProximitySensor(string broker)
	{
		var mqtt = new Charlotte(broker);

		mqtt.On["{room}/sensors/{sensor}"] = msg =>
		{
			Log("Data received from {0} (in {1}): {2}", msg.sensor, msg.room, msg.Message);
		};

		mqtt.On["sensors/bedroom/presence"] = async msg =>
		{
			if (msg.Message == "human present")
			{
				await mqtt.Publish("lights/bedroom", "on");
			}
		};
	}
```
