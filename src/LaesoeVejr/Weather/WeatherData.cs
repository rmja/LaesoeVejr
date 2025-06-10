namespace LaesoeVejr.Weather;

public record WeatherData
{
    public DateTime Time { get; init; }
    public required string StationId { get; init; }
    public double? WindSpeed { get; init; }
    public double? WindGust { get; init; }
    public double? WindDirection { get; init; }
    public double? AirTemperature { get; init; }
    public double? WaterTemperature { get; init; }
    public double? SeaLevel { get; init; }
    public double? AtmosphericPressure { get; init; }
    public double? Humidity { get; init; }
}
