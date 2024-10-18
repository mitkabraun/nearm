using System;
using System.Data.SqlClient;
using System.Linq;

namespace nearm_library.Extensions;

internal static class SqlReaderExtension
{
    public static int GoInt32(this SqlDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? 0 : reader.GetInt32(column);
    }

    public static byte[] GoByteArray(this SqlDataReader reader, int column)
    {
        return reader.IsDBNull(column) || reader.GetValue(column) is not byte[] bytes
            ? null
            : bytes;
    }

    public static string GoString(this SqlDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? null : reader.GetString(column);
    }

    public static DateTime GoDateTime(this SqlDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? DateTime.MinValue : reader.GetDateTime(column);
    }

    public static int[] GoInt32ArrayFromString(this SqlDataReader reader, int column)
    {
        var str = reader.GoString(column);
        return !string.IsNullOrEmpty(str)
            ? str.Split(',')
                .Select(s => int.TryParse(s, out var num) ? num : 0)
                .ToArray()
            : [];
    }
}
