namespace SlugStar.SqlServer
{
    public class SqlServerSlugStoreOptions
    {
        public SqlServerSlugStoreOptions()
        {
            //defaults
            TableSchema = "SlugStar";
            TableName = "Slugs";
        }

        public string TableName { get; set; }

        public string TableSchema { get; set; }
    }
}