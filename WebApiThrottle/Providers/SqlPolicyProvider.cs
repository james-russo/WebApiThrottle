using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebApiThrottle.Providers
{
    public class SqlPolicyProvider : IThrottlePolicyProvider
    {
        protected class Sql
        {
            private const string TableConfiguration = "[Throttler].[Configuration]";
            private const string TablePolicyRules = "[Throttler].[PolicyRule]";
            private const string TablePolicyTypes = "[Throttler].[PolicyType]";
            private const string TableWhitelist = "[Throttler].[Whitelist]";

            public static readonly string ConfigurationByServerName = $@"SELECT TOP 1 * FROM {TableConfiguration} WHERE ServerName = @serverName";

            public static readonly string RulesByServerName = $@"SELECT pt.Name, pr.Entry, pr.PerSecond, pr.PerDay, pr.PerMinute, pr.PerHour, pr.PerDay, pr.PerWeek
                                                                 FROM {TablePolicyRules} pr 
                                                                 INNER JOIN {TableConfiguration} c on c.Id = pr.ConfigurationId
                                                                 INNER JOIN {TablePolicyTypes} pt on pt.Id = pr.PolicyTypeId
                                                                 WHERE c.ServerName = @serverName";

            public static readonly string WhitelistByServerName = $@"SELECT pt.Name, wl.Entry FROM {TableWhitelist} wl
                                                                     INNER JOIN {TablePolicyTypes} pt on pt.Id = wl.PolicyTypeId
                                                                     INNER JOIN {TableConfiguration }c on c.Id = wl.ConfigurationId
                                                                     WHERE c.ServerName = @serverName";
        }

        private readonly string _connectionString;

        private readonly string _serverName;
        public SqlPolicyProvider(string connectionString, string serverName)
        {
            _connectionString = connectionString;
            _serverName = serverName;
        }

        public ThrottlePolicySettings ReadSettings()
        {
            var settings = new ThrottlePolicySettings();

            var sqlConnection = new SqlConnection(_connectionString);

            using (sqlConnection)
            {
                var sqlCommand = new SqlCommand(Sql.ConfigurationByServerName, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@serverName", _serverName);

                sqlConnection.Open();

                var reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    settings.IpThrottling = (bool)reader[nameof(settings.IpThrottling)];
                    settings.ClientThrottling = (bool)reader[nameof(settings.ClientThrottling)];
                    settings.EndpointThrottling = (bool)reader[nameof(settings.EndpointThrottling)];
                    settings.StackBlockedRequests = (bool)reader[nameof(settings.StackBlockedRequests)];
                    settings.LimitPerSecond = (long)reader[nameof(settings.LimitPerSecond)];
                    settings.LimitPerMinute = (long)reader[nameof(settings.LimitPerMinute)];
                    settings.LimitPerHour = (long)reader[nameof(settings.LimitPerHour)];
                    settings.LimitPerDay = (long)reader[nameof(settings.LimitPerDay)];
                    settings.LimitPerWeek = (long)reader[nameof(settings.LimitPerWeek)];

                }
                else
                {
                    //Should only be one record
                    settings = null;
                }

                sqlConnection.Close();
            }
            return settings;
        }

        public IEnumerable<ThrottlePolicyRule> AllRules()
        {
            var listOfRules = new List<ThrottlePolicyRule>();

            var sqlconnection = new SqlConnection(_connectionString);

            using (sqlconnection)
            {
                var sqlCommand = new SqlCommand(Sql.RulesByServerName, sqlconnection);

                sqlCommand.Parameters.AddWithValue("@serverName", _serverName);

                sqlconnection.Open();

                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    ThrottlePolicyType typeEnum;

                    Enum.TryParse(reader["Name"].ToString(), out typeEnum);

                    listOfRules.Add(new ThrottlePolicyRule()
                    {
                        PolicyType = typeEnum,
                        Entry = reader["Entry"].ToString(),
                        LimitPerSecond = GetValueOrDefault<long>(reader["PerSecond"]),
                        LimitPerMinute = GetValueOrDefault<long>(reader["PerMinute"]),
                        LimitPerHour = GetValueOrDefault<long>(reader["PerHour"]),
                        LimitPerDay = GetValueOrDefault<long>(reader["PerDay"]),
                        LimitPerWeek = GetValueOrDefault<long>(reader["PerWeek"])
                    });
                }

                sqlconnection.Close();
            }

            return listOfRules;
        }

        public IEnumerable<ThrottlePolicyWhitelist> AllWhitelists()
        {
            var listOfWhiteList = new List<ThrottlePolicyWhitelist>();

            var sqlconnection = new SqlConnection(_connectionString);

            using (sqlconnection)
            {
                var sqlCommand = new SqlCommand(Sql.WhitelistByServerName, sqlconnection);

                sqlCommand.Parameters.AddWithValue("@serverName", _serverName);

                sqlconnection.Open();

                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    ThrottlePolicyType typeEnum;

                    Enum.TryParse(reader["Name"].ToString(), out typeEnum);

                    listOfWhiteList.Add(new ThrottlePolicyWhitelist()
                    {
                        PolicyType = typeEnum,
                        Entry = reader["Entry"].ToString()
                    });
                }

                reader.Dispose();
                sqlconnection.Close();
            }

            return listOfWhiteList;
        }

        #region Helpers
        private T GetValueOrDefault<T>(object column)
        {
            return column == DBNull.Value ? default(T) : (T)column;
        }
        #endregion

    }
}
