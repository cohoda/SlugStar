using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using SlugStar.SqlServer;

namespace SlugStar.ConsoleAppSample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("This sample will create a slug for a piece of text, with 10000 iterations");
            Console.WriteLine("Please choose a sample to run: ");
            Console.WriteLine("");

            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Standard options (in memory)");
            Console.WriteLine("2) SQL Server Storage (requires connection string set in app.config)");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UseInMemoryStorage();
                    break;

                case "2":
                    UseSqlServerSlugStore();
                    break;

                default:
                    Console.WriteLine("Unknown choice");
                    break;
            }
        }

        private static void UseSqlServerSlugStore()
        {
            Console.Clear();


            SetupLocalDbIfRequired();

            var slugGenerator = new SlugGenerator(
                new SlugGeneratorOptions { IterationSeedValue = 1000 },
                new SqlServerSlugStore("DefaultConnection"));

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < 10000; i++)
            {
                var slug = slugGenerator.GenerateSlug("Some text that needs slugging " + i);
            }

            stopwatch.Stop();

            Console.WriteLine("Took " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("");
            Console.WriteLine("Press enter to return to main menu");
            Console.ReadLine();

            ShowMainMenu();
        }

        private static void UseInMemoryStorage()
        {
            Console.Clear();

            var slugGenerator = new SlugGenerator();

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < 10000; i++)
            {
                var slug = slugGenerator.GenerateSlug("Some text that needs slugging " + i);
            }

            stopwatch.Stop();

            Console.WriteLine("Took " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("");
            Console.WriteLine("Press enter to return to main menu");
            Console.ReadLine();

            ShowMainMenu();
        }

        private static void SetupLocalDbIfRequired()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            if (!connectionString.ConnectionString.Contains("localdb"))
                return;

            var dbFileName = AppDomain.CurrentDomain.BaseDirectory + "SlugStar.mdf";

            if (File.Exists(dbFileName))
                return;

            var connection = new SqlConnection("server=(localdb)\\mssqllocaldb");

            using (connection)
            {
                connection.Open();

                var dropDbSql = @"USE master
                    IF EXISTS(select * from sys.databases where name = 'SlugStar_Test')
                    DROP DATABASE SlugStar_Test";

                new SqlCommand(dropDbSql, connection).ExecuteNonQuery();

                var createDbSql = @"
                    CREATE DATABASE SlugStar_Test
                    ON PRIMARY (NAME=SlugStar_Test, FILENAME = '" + dbFileName + "')";

                new SqlCommand(createDbSql, connection).ExecuteNonQuery();
            }
        }
    }
}
