using EnergyManager.Data.Models;
using System.Data.SQLite;
using CsvHelper;
using Dapper;
using System.Globalization;
using CsvHelper.Configuration;

namespace EnergyManager.Data
{
    public class Seeder
    {
        public Seeder()
        {
            SQLiteConnection conn;

            string dataFile = DataHelper.GetDatabaseFile();
            // Create the database and associated tables

            if (!File.Exists(dataFile))
            {
                SQLiteConnection.CreateFile("EnergyDatabase.sqlite");

                string sql = @"CREATE TABLE Account (
                            AccountId INTEGER PRIMARY KEY,
                            FirstName TEXT NOT NULL,
                            LastName  TEXT NOT NULL
                        );";
                conn = DataHelper.GetConnection();
                conn.Execute(sql);
                
                sql = @"CREATE TABLE MeterReading (
                            AccountId INTEGER NOT NULL,
                            MeterReadingDateTime DATETIME NOT NULL,
                            MeterReadValue INTEGER NOT NULL,
                            CONSTRAINT fk_AccountId FOREIGN KEY (AccountId) REFERENCES Account (AccountId),
                            UNIQUE (AccountId, MeterReadingDateTime, MeterReadValue)
                        );";
                conn.Execute(sql);
            }
        }

        public void ImportAccounts(string accountFilename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };
            using var reader = new StreamReader(accountFilename);
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();
            
            while(csv.Read())
            {
                var record = csv.GetRecord<Account>();
                record.Persist();
            }
        }
    }
}