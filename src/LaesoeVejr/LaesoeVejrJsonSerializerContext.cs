using System.Text.Json.Serialization;
using LaesoeVejr.Weather;

namespace LaesoeVejr;

[JsonSerializable(typeof(List<WeatherData>))]
[JsonSerializable(typeof(List<AggregateWeatherData>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class LaesoeVejrJsonSerializerContext : JsonSerializerContext;
