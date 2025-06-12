using System.Data;
using System.Data.Common;
using Dapper;

namespace LaesoeVejr.Dapper;

public class UtcDateTimeHandler : TypeHandler<DateTime>
{
    public override void SetValue(DbParameter parameter, DateTime value)
    {
        parameter.Value = value.ToUniversalTime();
    }

    public override DateTime Parse(DbParameter parameter)
    {
        return DateTime.SpecifyKind((DateTime)parameter.Value!, DateTimeKind.Utc);
    }
}
