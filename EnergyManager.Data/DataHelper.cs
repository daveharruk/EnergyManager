using System.Data.SQLite;

namespace EnergyManager.Data
{
    public class DataHelper
    {
        public static string GetDatabaseFile()
        {
            string? dir = System.Environment.GetEnvironmentVariable("TEMP");
            if (dir == null)
            {
                throw new Exception("Cannot get environment variable TEMP for database");
            }

            return Path.Combine(dir, "EnergyDatabase.sqlite");
        }

        public static SQLiteConnection GetConnection()
        {
            string dataFile = GetDatabaseFile();
            return new SQLiteConnection($"Data Source={dataFile};Version=3;");
        }
    }
}