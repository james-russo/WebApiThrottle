using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApiThrottle.WebApiDemo.Helpers
{
    public class SqlLogger : IThrottleLogger
    {
        protected class Sql
        {
            private const string TableName = "[Throttler].[RequestLog]";

            private const string PRIMARY_KEY = "Id";

            public static readonly string Insert = $@"INSERT INTO {TableName} 
                                                    ([RequestId]
                                                    ,[ClientIp]
                                                    ,[ClientKey]
                                                    ,[Endpoint]
                                                    ,[TotalRequest]
                                                    ,[StartPeriod]
                                                    ,[RateLimit]
                                                    ,[RateLimitPeriod]
                                                    ,[LogDate]
                                                    ,[Request])
                                                VALUES
                                                    ( @requestid, @clientip, @clientkey, @endpoint, @totalrequest, @startperiod, @ratelimit, @ratelimitperiod, @logdate, @request)";
        }

        private readonly string _connectionString;

        public SqlLogger(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Log(ThrottleLogEntry entry)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var sqlcommand = new SqlCommand(Sql.Insert, conn))
                {
                    sqlcommand.Parameters.AddWithValue("@requestid", entry.RequestId);
                    sqlcommand.Parameters.AddWithValue("@clientip", entry.ClientIp);
                    sqlcommand.Parameters.AddWithValue("@clientkey", entry.ClientKey);
                    sqlcommand.Parameters.AddWithValue("@endpoint", entry.Endpoint);
                    sqlcommand.Parameters.AddWithValue("@totalrequest", entry.TotalRequests);
                    sqlcommand.Parameters.AddWithValue("@startperiod", entry.StartPeriod);
                    sqlcommand.Parameters.AddWithValue("@ratelimit", entry.RateLimit);
                    sqlcommand.Parameters.AddWithValue("@ratelimitperiod", entry.RateLimitPeriod);
                    sqlcommand.Parameters.AddWithValue("@logdate", entry.LogDate);
                    sqlcommand.Parameters.AddWithValue("@request", string.Empty);

                    conn.Open();

                    sqlcommand.ExecuteScalar();
                }
            }
        }
    }
}