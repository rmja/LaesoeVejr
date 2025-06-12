using Dapper;
using LaesoeVejr;
using LaesoeVejr.Cameras;
using LaesoeVejr.Dapper;
using LaesoeVejr.Weather;
using Microsoft.Extensions.FileProviders;
using QuestDB;

[assembly: DapperAot]
[assembly: TypeHandler<DateTime, UtcDateTimeHandler>]

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
builder.Services.AddTransient(services =>
    Sender.New(builder.Configuration.GetConnectionString("QuestDbSender")!)
);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolver = LaesoeVejrJsonSerializerContext.Default
);

builder
    .Services.AddHostedService<CameraImageDownloader>()
    .Configure<SftpOptions>(builder.Configuration.GetSection("Sftp"));

var app = builder.Build();

GetCameraImage.AddRoutes(app);
GetWeather.AddRoutes(app);
VesteroeHavnIngest.AddRoutes(app, app.Configuration["VesteroeHavnIngestUrl"]!);

app.MapWhen(
    x => !x.Request.Path.StartsWithSegments("/api"),
    notApi =>
        notApi
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapFallbackToFile("index.html"))
);

app.Run();

public partial class Program;
