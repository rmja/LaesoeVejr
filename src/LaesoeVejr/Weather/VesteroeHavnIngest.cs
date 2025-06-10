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
        CancellationToken cancellationToken = default
    )
    {
        sender.Table("laesoe_weather").Symbol("station_id", "vesteroe-havn");

        if (request.VindMidd is >= 0 and <= 150)
        {
            sender.Column("wind_speed", request.VindMidd.Value);
        }

        if (request.VindStod is >= 0 and <= 200)
        {
            sender.Column("wind_gust", request.VindStod.Value);
        }

        if (request.VindRetn is >= 0 and <= 360)
        {
            sender.Column("wind_dir", request.VindRetn.Value);
        }

        if (request.LuftTemp is >= -50 and <= 60)
        {
            sender.Column("air_temp", request.LuftTemp.Value);
        }

        if (request.VandTemp is >= -20 and <= 40)
        {
            sender.Column("water_temp", request.VandTemp.Value);
        }

        if (request.VandStan is >= -20 and <= 20)
        {
            sender.Column("sea_level", request.VandStan.Value);
        }

        if (request.LuftTryk is >= 500 and <= 1500)
        {
            sender.Column("pressure", request.LuftTryk.Value);
        }

        if (request.LuftFugt is >= 0 and <= 100)
        {
            sender.Column("humidity", request.LuftFugt.Value);
        }

        await sender.AtAsync(DateTime.UtcNow, cancellationToken);
    }
}
