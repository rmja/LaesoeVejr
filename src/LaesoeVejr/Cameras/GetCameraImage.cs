using Microsoft.Extensions.Options;

namespace LaesoeVejr.Cameras;

public static class GetCameraImage
{
    public static void AddRoutes(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/cameras/{cameraId}/image.jpg", GetImageAsync);
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
}
