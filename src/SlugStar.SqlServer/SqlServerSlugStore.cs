using System;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using SlugStar.SlugStore;

namespace SlugStar.SqlServer
{
    public class SqlServerSlugStore : ISlugStore
    {
        private readonly SqlServerSlugStoreOptions _options;
        private readonly string _connectionString;

        public SqlServerSlugStore(string nameOrConnectionString) 
            : this(nameOrConnectionString, new SqlServerSlugStoreOptions())
        { }

        public SqlServerSlugStore(string nameOrConnectionString, SqlServerSlugStoreOptions options = null)
        {
            if (options == null)
                options = new SqlServerSlugStoreOptions();

            _options = options;

            if (nameOrConnectionString == null) throw new ArgumentNullException("nameOrConnectionString");

            if (IsConnectionString(nameOrConnectionString))
            {
                _connectionString = nameOrConnectionString;
            }
            else if (IsConnectionStringInConfiguration(nameOrConnectionString))
            {
                _connectionString = ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;
            }
            else
            {
                throw new ArgumentException($"Could not find connection string with name '{nameOrConnectionString}' in application config file");
            }

            Installer.InstallSqlTable(_connectionString, _options);
        }

        private bool IsConnectionString(string nameOrConnectionString)
        {
            return nameOrConnectionString.Contains(";");
        }

        private bool IsConnectionStringInConfiguration(string connectionStringName)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];

            return connectionStringSetting != null;
        }

        public bool Exists(string slug)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var query = $"select count(slug) from [{_options.TableSchema}].[{_options.TableName}] where [Slug] = @slug";

                var slugResult = sqlConnection.ExecuteScalar<int>(query, new { slug });

                return slugResult > 0;
            }
        }

        public void Store(Slug slug)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var command = $"insert into [{_options.TableSchema}].[{_options.TableName}]([Slug],[Created]) values (@Slug,@Created)";

                sqlConnection.Execute(command, new
                {
                    Slug = slug.Value,
                    slug.Created
                });

                sqlConnection.Close();
            }
        }
    }
}
