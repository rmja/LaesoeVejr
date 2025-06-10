using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace LaesoeVejr;

public class CameraImageDownloader(
    IHttpClientFactory httpClientFactory,
    IOptions<SftpOptions> options,
    TimeProvider timeProvider
) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        await DownloadCameraImagesAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5), timeProvider);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DownloadCameraImagesAsync(stoppingToken);
        }
    }

    private async Task DownloadCameraImagesAsync(CancellationToken cancellationToken)
    {
        using var httpClient = httpClientFactory.CreateClient();
        await Task.WhenAll(
            GetLegacyCameraImageAsync(
                httpClient,
                "Vejrstation.jpg",
                "laesoefaergen-oest.jpg",
                cancellationToken
            ),
            GetLegacyCameraImageAsync(
                httpClient,
                "VejrstationVest.jpg",
                "laesoefaergen-vest.jpg",
                cancellationToken
            ),
            GetLegacyCameraImageAsync(
                httpClient,
                "HavnekontorSyd.jpg",
                "havnekontoret-syd.jpg",
                cancellationToken
            ),
            GetLegacyCameraImageAsync(
                httpClient,
                "HavnekontorNord.jpg",
                "havnekontoret-nord.jpg",
                cancellationToken
            ),
            GetLegacyCameraImageAsync(
                httpClient,
                "telemast.jpg",
                "flyvepladsen-telemast.jpg",
                cancellationToken
            ),
            GetLegacyCameraImageAsync(
                httpClient,
                "vindpose.jpg",
                "flyvepladsen-vindpose.jpg",
                cancellationToken
            )
        );
        //using var client = new SftpClient(
        //    options.Value.Host,
        //    options.Value.Port,
        //    options.Value.Username,
        //    options.Value.Password
        //);

        //await client.ConnectAsync(cancellationToken);

        //await foreach (
        //    var file in client.ListDirectoryAsync("/home/laesoe-vejr", cancellationToken)
        //)
        //{
        //    if (file.Name == "Vejrstation.jpg")
        //    {
        //        await DownloadImageAsync(
        //            client,
        //            file.FullName,
        //            "laesoefaergen-vest.jpg",
        //            cancellationToken
        //        );
        //    }
        //}
    }

    private static async Task GetLegacyCameraImageAsync(
        HttpClient client,
        string legacyFilename,
        string fileName,
        CancellationToken cancellationToken
    )
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://93.167.192.205/Camera/{legacyFilename}"
        );
        request.Headers.Host = "www.laesoe-vejr.dk";
        using var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var tmpFileName = Path.GetTempFileName();
        await using (var tmpFile = File.OpenWrite(tmpFileName))
        {
            await response.Content.CopyToAsync(tmpFile, cancellationToken);
        }

        File.Move(
            tmpFileName,
            Path.Combine(Path.GetTempPath(), "LaesoeVejrCameraImages", fileName),
            overwrite: true
        );
    }

    private static async Task DownloadImageAsync(
        SftpClient client,
        string remoteFilePath,
        string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var remoteFile = await client.OpenAsync(
            remoteFilePath,
            FileMode.Open,
            FileAccess.Read,
            cancellationToken
        );

        var tmpFileName = Path.GetTempFileName();
        await using (var tmpFile = File.OpenWrite(tmpFileName))
        {
            await remoteFile.CopyToAsync(tmpFile, cancellationToken);
        }

        File.Move(
            tmpFileName,
            Path.Combine(Path.GetTempPath(), "LaesoeVejrCameraImages", fileName),
            overwrite: true
        );

        //await file.DeleteAsync(cancellationToken);
    }
}
