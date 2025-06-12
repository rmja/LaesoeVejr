namespace LaesoeVejr.Cameras;

public static class GetCameraImage
{
    public static void AddRoutes(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/cameras/{cameraId}/image.jpg", GetImageAsync);
        builder.MapGet("/api/cameras/{cameraId}/thumbnail.jpg", GetThumbnailAsync);
    }

    public static IResult GetImageAsync(
        string cameraId,
        IWebHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        var fileName = $"{cameraId}.jpg";

        if (!hostEnvironment.WebRootFileProvider.GetFileInfo(fileName).Exists)
        {
            return Results.NotFound();
        }
        return Results.File(fileName, contentType: "image/jpeg", fileDownloadName: fileName);
    }

    public static IResult GetThumbnailAsync(
        string cameraId,
        IWebHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default
    )
    {
        var fileName = $"{cameraId}-thumbnail.jpg";

        if (!hostEnvironment.WebRootFileProvider.GetFileInfo(fileName).Exists)
        {
            return Results.NotFound();
        }
        return Results.File(fileName, contentType: "image/jpeg", fileDownloadName: fileName);
    }
}
