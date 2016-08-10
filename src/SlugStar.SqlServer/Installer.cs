using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Dapper;

namespace SlugStar.SqlServer
{
    internal class Installer
    {
        public static void InstallSqlTable(string connectionString, SqlServerSlugStoreOptions options)
        {
            var script = GetStringResource(typeof(SqlServerSlugStore).Assembly, "SlugStar.SqlServer.Install.sql");

            //do replaces
            script = script.Replace("$(SCHEMA_NAME)", options.TableSchema);
            script = script.Replace("$(TABLE_NAME)", options.TableName);

            using (var connection = new SqlConnection(connectionString))
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        connection.Execute(script);
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.ErrorCode == 1205)
                        {
                            Trace.WriteLine("Deadlock occurred during automatic migration execution. Retrying...");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private static string GetStringResource(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(String.Format(
                        "Requested resource `{0}` was not found in the assembly `{1}`.",
                        resourceName,
                        assembly));
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
