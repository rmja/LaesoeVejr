using System.Data.Common;
using QuestDB.Enums;
using QuestDB.Utils;

namespace LaesoeVejr;

record NativeAotSenderOptions : SenderOptions
{
    public NativeAotSenderOptions(string connectionString)
    {
        var parts = connectionString.Split("::");

        var builder = new DbConnectionStringBuilder() { ConnectionString = parts[1] };
        builder.Add("protocol", parts[0]);

        // https://github.com/questdb/net-questdb-client/blob/1fe7650c1e707a90d6a8dac24b78445b1449bca3/src/net-questdb-client/Utils/SenderOptions.cs#L85-L106
        protocol = builder.GetEnumOrNull<ProtocolType>("protocol") ?? protocol;
        addr = builder.GetStringOrNull("addr") ?? addr;
    }
}

static class DbConnectionStringBuilderExtensions
{
    public static string? GetStringOrNull(this DbConnectionStringBuilder builder, string key)
    {
        if (!builder.TryGetValue(key, out var value))
        {
            return null!;
        }

        return (string)value;
    }

    public static TEnum? GetEnumOrNull<TEnum>(
        this DbConnectionStringBuilder builder,
        string key,
        bool ignoreCase = false
    )
        where TEnum : struct, Enum
    {
        if (!builder.TryGetValue(key, out var value))
        {
            return null!;
        }

        return Enum.Parse<TEnum>((string)value, ignoreCase);
    }
}
