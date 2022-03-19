using Dapper;

namespace EnergyManager.Data.Models
{
    public class RawMeterReading
    {
        public string AccountId { get; set; }
        public string MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }

        public RawMeterReading(string accountId, string meterReadingDateTime, string meterReadValue)
        {
            AccountId = accountId;
            MeterReadingDateTime = meterReadingDateTime;
            MeterReadValue = meterReadValue;
        }

        public bool Valid()
        {
            // Check whether the data is of the appropriate types
            if (!int.TryParse(AccountId, out int accountId) ||
                !DateTime.TryParse(MeterReadingDateTime, out DateTime meterReadingDateTime) ||
                !int.TryParse(MeterReadValue, out int meterReadValue))
            {
                return false;
            }

            // Check that the meter reading itself is sane
            if (meterReadValue < 0 || meterReadValue > 99999)
            {
                return false;
            }

            // Try to save - if it fails then something else was wrong (e.g. the accountid didn't exist or the same record was already loaded)
            var newRecord = new MeterReading(accountId, meterReadingDateTime, meterReadValue);

            // Check current meter reading against the latest that already exists
            DateTime? latestExistingReadingTime = newRecord.LatestExistingReading();
            if (latestExistingReadingTime.HasValue && newRecord.MeterReadingDateTime < latestExistingReadingTime)
            {
                return false;
            }

            if (!newRecord.TryPersist())
            {
                return false;
            }

            return true;
        }
    }
}