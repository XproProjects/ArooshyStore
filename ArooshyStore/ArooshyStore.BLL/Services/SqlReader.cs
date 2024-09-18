using System;
using System.Data.SqlClient;

namespace ArooshyStore.BLL.Services
{
    public static class SqlReader
    {
        public static DateTime? SafeGetDateTime(this SqlDataReader reader, string columnName)
        {
            //Get Ordinal returns index of column
            int columnIndex = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(columnIndex))
            {
                return Convert.ToDateTime(reader[columnName]);
            }
            else
            {
                return null;
            }
        }
    }
}
