using System;
using System.Data.SqlClient;

namespace ArooshyStore.BLL.Services
{
    public sealed class ErrorHandler
    {
        private static ErrorHandler instance = null;
        private static readonly object lockThread = new object();
        ErrorHandler() { }
        public static ErrorHandler GetInstance
        {
            get
            {
                //It will check if instance is null then will go further
                if (instance == null)
                {
                    //It will lock all other threads if they are accessing this class at the same time to prevent multiple instances in different threads
                    lock (lockThread)
                    {
                        //It will check if instance is null then create an instance
                        if (instance == null)
                        {
                            instance = new ErrorHandler();
                        }
                    }
                }
                return instance;
            }
        }
        public void InsertError(int? userId, string errorDescription, string errorSource,
                                        string errorClass, string errorAction)
        {
            try
            {
                errorDescription = GetValue(errorDescription);
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "insert into tblErrorsLog(ErrorLineNumber,ErrorDescription,ErrorSource," +
                                        "ErrorClass,ErrorAction,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) " +
                                        "values('Code Exception',@ErrorDescription,@ErrorSource,@ErrorClass," +
                                        "@ErrorAction,CURRENT_TIMESTAMP,@UserId,CURRENT_TIMESTAMP,@UserId)";
                    cmd.Parameters.AddWithValue("@ErrorDescription", errorDescription);
                    cmd.Parameters.AddWithValue("@ErrorSource", errorSource);
                    cmd.Parameters.AddWithValue("@ErrorClass", errorClass);
                    cmd.Parameters.AddWithValue("@ErrorAction", errorAction);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    SqlConnection.ClearPool(con);
                }
            }
            catch (Exception ex) { }
        }
        public string GetValue(string value)
        {
            value = value.Replace("'", "''");
            return value;
        }
    }
}
