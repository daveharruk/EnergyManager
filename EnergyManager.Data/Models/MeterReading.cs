using Dapper;

namespace EnergyManager.Data.Models
{
    public class MeterReading
    {
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }

        public MeterReading(int accountId, DateTime meterReadingDateTime, int meterReadValue)
        {
            AccountId = accountId;
            MeterReadingDateTime = meterReadingDateTime;
            MeterReadValue = meterReadValue;
        }

        public MeterReading()
        {
            // Required for Dapper object initialisation
        }

        public bool TryPersist()
        {
            var conn = DataHelper.GetConnection();
            var parameters = new DynamicParameters(this);
            try
            {
                conn.Execute("insert into MeterReading(AccountId, MeterReadingDateTime, MeterReadValue) values(@AccountId, @MeterReadingDateTime, @MeterReadValue)", parameters);
            }
            catch
            {
                // TODO: Add logging of the exception
                return false;
            }
            return true;
        }

        public DateTime? LatestExistingReading()
        {
            var conn = DataHelper.GetConnection();
            var parameters = new DynamicParameters(this);
            return conn.QueryFirst<DateTime?>("select max(MeterReadingDateTime) from MeterReading where AccountId = @AccountId", parameters);
        }

        public static List<MeterReading> GetAll()
        {
            var conn = DataHelper.GetConnection();
            return conn.Query<MeterReading>("select * from MeterReading").ToList();
        }

        public int Delete()
        {
            var conn = DataHelper.GetConnection();
            var parameters = new DynamicParameters(this);
            return conn.Execute("delete from MeterReading where AccountId = @AccountId and MeterReadingDateTime = @MeterReadingDateTime and MeterReadValue = @MeterReadValue", parameters);
        }
    }
}
