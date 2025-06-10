namespace LaesoeVejr.Tests;

public class WeatherTests(AppFactory factory) : IClassFixture<AppFactory>
{
    [Fact]
    public async Task CanIngest()
    {
        using var client = factory.CreateClient();

        await client.GetAsync(
            "/Ingest?LuftTryk=850&LuftTemp=6.5&LuftFugt=-0.1&VandStan=-0.13&VandTemp=8.6&VindMidd=6.3&VindRetn=50.6&VindStod=8.2",
            TestContext.Current.CancellationToken
        );
    }
}
