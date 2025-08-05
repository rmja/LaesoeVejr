using MessagePipe;
using QuestDB.Senders;

namespace LaesoeVejr.Weather;

public static class VesteroeHavnIngest
{
    public record Request
    {
        public double? VindMidd { get; init; }
        public double? VindStod { get; init; }
        public double? VindRetn { get; init; }
        public double? LuftTemp { get; init; }
        public double? VandTemp { get; init; }
        public double? VandStan { get; init; }
        public double? LuftTryk { get; init; }
        public double? LuftFugt { get; init; }
    }

    public static void AddRoutes(IEndpointRouteBuilder builder, string ingestPath)
    {
        builder.MapGet(ingestPath, HandleAsync);
    }

    private static async Task HandleAsync(
        [AsParameters] Request request,
        ISender sender,
        IAsyncPublisher<WeatherData> publisher,
        CancellationToken cancellationToken = default
    )
    {
        var data = new WeatherData()
        {
            Time = DateTime.UtcNow,
            StationId = "vesteroe-havn",
            WindSpeed = request.VindMidd is >= 0 and <= 150 ? request.VindMidd : null,
            WindGust = request.VindStod is >= 0 and <= 200 ? request.VindStod : null,
            WindDirection = request.VindRetn is >= 0 and <= 360 ? request.VindRetn : null,
            AirTemperature = request.LuftTemp is >= -50 and <= 60 ? request.LuftTemp : null,
            WaterTemperature = request.VandTemp is >= -20 and <= 40 ? request.VandTemp : null,
            SeaLevel = request.VandStan is >= -20 and <= 20 ? request.VandStan : null,
            AtmosphericPressure = request.LuftTryk is >= 500 and <= 1500 ? request.LuftTryk : null,
            Humidity = request.LuftFugt is >= 0 and <= 100 ? request.LuftFugt : null,
        };

        await sender
            .Table("laesoe_weather")
            .Symbol("station_id", data.StationId)
            .NullableColumn("wind_speed", data.WindSpeed)
            .NullableColumn("wind_gust", data.WindGust)
            .NullableColumn("wind_dir", data.WindDirection)
            .NullableColumn("air_temp", data.AirTemperature)
            .NullableColumn("water_temp", data.WaterTemperature)
            .NullableColumn("sea_level", data.SeaLevel)
            .NullableColumn("pressure", data.AtmosphericPressure)
            .NullableColumn("humidity", data.Humidity)
            .AtAsync(DateTime.UtcNow, cancellationToken);

        await sender.SendAsync(cancellationToken);

        await publisher.PublishAsync(data, CancellationToken.None);
    }
}
