using CompressedStaticFiles;
using Dapper;
using LaesoeVejr;
using LaesoeVejr.Cameras;
using LaesoeVejr.Dapper;
using LaesoeVejr.Weather;
using MessagePipe;
using Microsoft.Extensions.FileProviders;
using QuestDB;
using QuestDB.Utils;

[module: DapperAot]
[module: TypeHandler<DateTime, UtcDateTimeHandler>] // This does not work so we manually specify kind in the exposed models

var builder = WebApplication.CreateSlimBuilder(args);

var cameraImagesDirectory = new DirectoryInfo(
    Path.Combine(Path.GetTempPath(), "LaesoeVejrCameraImages")
);
if (!cameraImagesDirectory.Exists)
{
    cameraImagesDirectory.Create();
}
builder.Environment.WebRootFileProvider = new CompositeFileProvider(
    new PhysicalFileProvider(cameraImagesDirectory.FullName),
    builder.Environment.WebRootFileProvider
);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("QuestDbPostgresql")!
);

var senderOptions = new SenderOptions(builder.Configuration.GetConnectionString("QuestDbSender")!);
builder.Services.AddTransient(services => Sender.New(senderOptions));

builder
    .Services.AddResponseCompression(options => options.EnableForHttps = true)
    .AddCompressedStaticFiles()
    .ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.TypeInfoResolver = LaesoeVejrJsonSerializerContext.Default
    );

builder
    .Services.AddMessagePipe()
    .Services.AddSingleton<AsyncMessageBroker<WeatherData>>()
    .AddSingleton<AsyncMessageBrokerCore<WeatherData>>();

builder
    .Services.AddHostedService<CameraImageDownloader>()
    .Configure<SftpOptions>(builder.Configuration.GetSection("Sftp"));

var app = builder.Build();

app.UseResponseCompression();
GetCameraImage.AddRoutes(app);
GetWeather.AddRoutes(app);
VesteroeHavnIngest.AddRoutes(app, app.Configuration["VesteroeHavnIngestUrl"]!);

app.MapWhen(
    x => !x.Request.Path.StartsWithSegments("/api"),
    notApi =>
        notApi
            .UseDefaultFiles()
            .UseCompressedStaticFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapFallbackToFile("index.html"))
);

app.Run();

public partial class Program;
