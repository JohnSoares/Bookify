using System.Data;
using System.Globalization;
using Dapper;

namespace Bookify.Infrastructure.Data;

internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
    {
        if (value is DateOnly d)
        {
            return d;
        }
        if (value is DateTime dt)
        {
            return DateOnly.FromDateTime(dt);
        }

        throw new DataException($"Cannot convert value of type '{value.GetType()}' to DateOnly");
    }

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value; 
    }
}
