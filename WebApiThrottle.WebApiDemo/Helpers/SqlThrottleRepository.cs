﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebApiThrottle.WebApiDemo.Helpers
{
    public class SqlThrottleRepository : IThrottleRepository
    {
        protected class Sql
        {
            private const string TableName = "[Throttler].[RequestCounter]";

            private const string PrimaryKey = "Id";

            public static readonly string FirstOrDefault = $"SELECT TOP 1 * FROM {TableName} WHERE {PrimaryKey} = @id";

            public static readonly string Save = $@" IF EXISTS(SELECT * FROM {TableName} WHERE Id = @id)" +
                                              $" UPDATE {TableName} SET TotalRequests = @totalrequests, Timestamp = @timestamp, ExpirationTime = @ExpirationTime" +
                                               " ELSE " +
                                              $" INSERT INTO {TableName} (Id, Timestamp, TotalRequests, ExpirationTime, ClientKey) VALUES (@id, @timestamp, @totalrequests, @ExpirationTime, @clientKey)";

            public static readonly string Any = $"SELECT COUNT('x') FROM {TableName} WHERE {PrimaryKey} = @id";


            public static readonly string Delete = $"DELETE FROM {TableName} WHERE Id = @id";

            public static readonly string Truncate = $"TRUNCATE TABLE {TableName}";

            internal static readonly string SchemaCheck = $"SELECT COUNT('x') FROM information_schema.schemata WHERE schema_name = 'Throttler'";

        }

        public SqlThrottleRepository Migrate()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Must be called after initialization");

            using (var sqlConn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(Sql.SchemaCheck, sqlConn))
                {
                    sqlConn.Open();

                    var result = command.ExecuteScalar();

                    if ((int)result > 0)
                        return this;
                }
            }


            var assembly = typeof(IThrottleRepository).Assembly;

            var resourceName = "WebApiThrottle.Resources.Sql_Schema.sql";

            var sql = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    sql = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(sql))
                throw new Exception("An error occured generating sql");

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string[] batches = sql.Split(new[] { "GO" + Environment.NewLine }, StringSplitOptions.None);

                foreach (string batch in batches)
                {
                    if (!string.IsNullOrEmpty(batch))
                    {
                        using (var sqlCommand = new SqlCommand(batch, sqlConnection))
                        {
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            return this;
        }


        private readonly string _connectionString;

        public SqlThrottleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Any(string id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(Sql.Any, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    sqlConnection.Open();

                    var result = (int)sqlCommand.ExecuteScalar();

                    return result > 0;
                }
            }
        }

        public ThrottleCounter? FirstOrDefault(string id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(Sql.FirstOrDefault, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    sqlConnection.Open();

                    var reader = sqlCommand.ExecuteReader();

                    if (!reader.HasRows)
                        return null;

                    reader.Read();

                    return new ThrottleCounter()
                    {
                        Timestamp = Convert.ToDateTime(reader["Timestamp"]),
                        TotalRequests = Convert.ToInt64(reader["TotalRequests"])
                    };

                }
            }
        }

        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime, string clientKey)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(Sql.Save, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);
                    sqlCommand.Parameters.AddWithValue("@timestamp", throttleCounter.Timestamp);
                    sqlCommand.Parameters.AddWithValue("@totalrequests", throttleCounter.TotalRequests);
                    sqlCommand.Parameters.AddWithValue("@ExpirationTime", expirationTime.Ticks);
                    sqlCommand.Parameters.AddWithValue("@clientKey", clientKey);

                    sqlConnection.Open();

                    sqlCommand.ExecuteScalar();
                }
            }
        }

        public void Remove(string id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(Sql.Delete, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    sqlConnection.Open();

                    sqlCommand.ExecuteScalar();
                }
            }
        }

        public void Clear()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(Sql.Truncate, sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.ExecuteScalar();
                }
            }
        }
    }
}