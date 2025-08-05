using System.ComponentModel.DataAnnotations;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using Dapper;
using MessagePipe;
using Npgsql;

namespace LaesoeVejr.Weather;

public static class GetWeather
{
    public static void AddRoutes(IEndpointRouteBuilder builder)
    {
        var weather = builder.MapGroup("/api/stations/{stationId}/weather");
        weather.MapGet("/", GetWeatherAsync);
        weather.MapGet("/events", GetWeatherEventsAsync);
        weather.MapGet("/history", GetWeatherHistoryAsync);
    }

    private static async Task<IResult> GetWeatherAsync(
        string stationId,
        NpgsqlConnection db,
        [Range(1, 1000)] int limit = 1000,
        CancellationToken cancellationToken = default
    )
    {
        var sql = typeof(GetWeather).GetResourceString("GetWeather.sql");
        var weather = await db.QueryAsync<WeatherData>(sql, new { stationId, limit });
        return TypedResults.Ok(weather.AsList());
    }

    private static IResult GetWeatherEventsAsync(
        string stationId,
        HttpContext httpContext,
        IAsyncSubscriber<WeatherData> subscriber,
        CancellationToken cancellationToken = default
    )
    {
        async IAsyncEnumerable<SseItem<WeatherData>> StreamWeatherEvents(
            [EnumeratorCancellation] CancellationToken cancellationToken
        )
        {
            await foreach (
                var weather in subscriber.AsAsyncEnumerable().WithCancellation(cancellationToken)
            )
            {
                yield return new SseItem<WeatherData>(weather);
            }
        }

        httpContext.Response.Headers["X-Accel-Buffering"] = "no";

        return TypedResults.ServerSentEvents(StreamWeatherEvents(cancellationToken));
    }

    public record Request(DateTimeOffset Start, DateTimeOffset End, string Step);

    private static async Task<IResult> GetWeatherHistoryAsync(
        string stationId,
        [AsParameters] Request request,
        NpgsqlConnection db,
        CancellationToken cancellationToken = default
    )
    {
        var view = request.Step switch
        {
            "month" => "laesoe_weather_month",
            "day" => "laesoe_weather_day",
            "hour" => "laesoe_weather_hour",
            "5min" => "laesoe_weather_5min",
            _ => null,
        };
        if (view is null)
        {
            return Results.BadRequest();
        }
        var sql = typeof(GetWeather)
            .GetResourceString("GetWeatherHistory.sql")
            .Replace("{{view}}", view);
        var history = await db.QueryAsync<AggregateWeatherData>(
            sql,
            new
            {
                stationId,
                start = request.Start.UtcDateTime,
                end = request.End.UtcDateTime,
            }
        );
        return TypedResults.Ok(history.AsList());
    }
}
