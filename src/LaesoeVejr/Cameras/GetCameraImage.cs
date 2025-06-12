namespace LaesoeVejr.Cameras;

public static class GetCameraImage
{
    public static void AddRoutes(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/cameras/{cameraId}/{basename}.{extension}", GetImageAsync);
    }

    public static IResult GetImageAsync(
        string cameraId,
        string basename,
        string extension,
        IWebHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        var fileName = $"{basename}.{extension}" switch
        {
            "thumbnail.webp" => $"{cameraId}-thumbnail.webp",
            "preview.webp" => $"{cameraId}-preview.webp",
            "image.jpg" => $"{cameraId}.jpg",
            _ => null,
        };

        if (fileName is null || !hostEnvironment.WebRootFileProvider.GetFileInfo(fileName).Exists)
        {
            return Results.NotFound();
        }
        return Results.File(fileName, fileDownloadName: fileName);
    }
}
