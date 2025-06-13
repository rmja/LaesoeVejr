using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LaesoeVejr;

public class CameraImageDownloader(
    ILogger<CameraImageDownloader> logger,
    IOptions<SftpOptions> options,
    TimeProvider timeProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DownloadImagesAsync(stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5), timeProvider);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DownloadImagesAsync(stoppingToken);
        }
    }

    private async Task DownloadImagesAsync(CancellationToken cancellationToken)
    {
        using var client = new SftpClient(
            options.Value.Host,
            options.Value.Port,
            options.Value.Username,
            options.Value.Password
        );

        await client.ConnectAsync(cancellationToken);

        await TryDownloadImageAsync(
            client,
            "laesoefaergen-oest",
            "Camera/Vejrstation",
            cancellationToken
        );
        await TryDownloadImageAsync(
            client,
            "laesoefaergen-vest",
            "Camera/VejrstationVest",
            cancellationToken
        );
        await TryDownloadImageAsync(
            client,
            "havnekontoret-syd",
            "Camera/HavnekontorSyd",
            cancellationToken
        );
        await TryDownloadImageAsync(
            client,
            "havnekontoret-nord",
            "Camera/HavnekontorNord",
            cancellationToken
        );
        await TryDownloadImageAsync(
            client,
            "flyvepladsen-telemast",
            "Camera/Telemast",
            cancellationToken
        );

        await TryDownloadImageAsync(
            client,
            "flyvepladsen-vindpose",
            "Camera/Vindpose",
            cancellationToken
        );
    }

    private async Task TryDownloadImageAsync(
        SftpClient client,
        string cameraId,
        string remoteDirectory,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await DownloadImageAsync(client, cameraId, remoteDirectory, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download image for camera {CameraId}", cameraId);
        }
    }

    private async Task DownloadImageAsync(
        SftpClient client,
        string cameraId,
        string remoteDirectory,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Downloading image for camera {CameraId} from {RemoteDirectory}",
            cameraId,
            remoteDirectory
        );

        var images = new List<ISftpFile>();
        await foreach (var file in client.ListDirectoryAsync(remoteDirectory, cancellationToken))
        {
            if (file.Name.EndsWith(".jpg"))
            {
                images.Add(file);
            }
        }

        var newestImage = images.MaxBy(f => f.LastWriteTime);
        if (newestImage is null)
        {
            return;
        }

        await using var remoteFile = await client.OpenAsync(
            newestImage.FullName,
            FileMode.Open,
            FileAccess.Read,
            cancellationToken
        );

        var tempOriginalJpgFileName = Path.GetTempFileName();
        await using (var tmpFile = File.OpenWrite(tempOriginalJpgFileName))
        {
            await remoteFile.CopyToAsync(tmpFile, cancellationToken);
        }

        var tempPreviewWebpFileName = Path.GetTempFileName();
        var tempThumbnailWebpFileName = Path.GetTempFileName();
        using (var image = await Image.LoadAsync(tempOriginalJpgFileName, cancellationToken))
        {
            var preview = image.Clone(image =>
                image.Resize(
                    new ResizeOptions() { Mode = ResizeMode.Max, Size = new Size(1116, 900) }
                )
            );
            await preview.SaveAsWebpAsync(tempPreviewWebpFileName, cancellationToken);

            var thumbnail = image.Clone(image =>
                image.Resize(
                    new ResizeOptions() { Mode = ResizeMode.Max, Size = new Size(634, 500) }
                )
            );
            await thumbnail.SaveAsWebpAsync(tempThumbnailWebpFileName, cancellationToken);
        }

        File.Move(
            tempOriginalJpgFileName,
            Path.Combine(Path.GetTempPath(), "LaesoeVejrCameraImages", cameraId + ".jpg"),
            overwrite: true
        );

        File.Move(
            tempPreviewWebpFileName,
            Path.Combine(Path.GetTempPath(), "LaesoeVejrCameraImages", cameraId + "-preview.webp"),
            overwrite: true
        );

        File.Move(
            tempThumbnailWebpFileName,
            Path.Combine(
                Path.GetTempPath(),
                "LaesoeVejrCameraImages",
                cameraId + "-thumbnail.webp"
            ),
            overwrite: true
        );

        // Delete all but the newest image
        foreach (var image in images)
        {
            if (image != newestImage)
            {
                await image.DeleteAsync(cancellationToken);
            }
        }
    }
}
