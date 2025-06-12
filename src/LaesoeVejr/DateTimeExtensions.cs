namespace LaesoeVejr;

internal static class DateTimeExtensions
{
    public static DateTime AsUtc(this DateTime dateTime) =>
        dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => throw new ArgumentOutOfRangeException(nameof(dateTime), "Invalid DateTime kind"),
        };
}
