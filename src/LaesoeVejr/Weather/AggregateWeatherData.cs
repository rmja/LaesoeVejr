using Dapper;
using LaesoeVejr.Dapper;

namespace LaesoeVejr.Weather;

[TypeHandler<DateTime, UtcDateTimeHandler>]
public record AggregateWeatherData
{
    private DateTime _time;

    public DateTime Time
    {
        get => _time;
        init => _time = value.AsUtc();
    }
    public required string StationId { get; init; }
    public double? WindSpeedAvg { get; init; }
    public double? WindSpeedMin { get; init; }
    public double? WindSpeedMax { get; init; }
    public double? WindGustAvg { get; init; }
    public double? WindGustMin { get; init; }
    public double? WindGustMax { get; init; }
    public double? WindDirectionAvg { get; init; }
    public double? AirTemperatureAvg { get; init; }
    public double? AirTemperatureMin { get; init; }
    public double? AirTemperatureMax { get; init; }
    public double? WaterTemperatureAvg { get; init; }
    public double? WaterTemperatureMin { get; init; }
    public double? WaterTemperatureMax { get; init; }
    public double? SeaLevelAvg { get; init; }
    public double? SeaLevelMin { get; init; }
    public double? SeaLevelMax { get; init; }
    public double? AtmosphericPressureAvg { get; init; }
    public double? AtmosphericPressureMin { get; init; }
    public double? AtmosphericPressureMax { get; init; }
    public double? HumidityAvg { get; init; }
    public double? HumidityMin { get; init; }
    public double? HumidityMax { get; init; }
}
