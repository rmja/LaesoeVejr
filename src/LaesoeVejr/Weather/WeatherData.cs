using Dapper;

namespace LaesoeVejr.Weather;

public record WeatherData
{
    private DateTime _time;

    public DateTime Time
    {
        get => _time;
        init => _time = value.AsUtc();
    }
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
