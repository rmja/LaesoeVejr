namespace LaesoeVejr;

public static class TypeExtensions
{
    public static string GetResourceString(this Type type, string fileName)
    {
        var resourceName = $"{type.Namespace}.{fileName}";
        var stream =
            type.Assembly.GetManifestResourceStream(resourceName)
            ?? throw new ArgumentException(
                $"Resource '{resourceName}' not found in assembly '{type.Assembly.FullName}'."
            );
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
