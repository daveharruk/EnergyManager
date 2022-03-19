using EnergyManager.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace EnergyManager.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethodInit()
        {
            // Assumption is that account data has already been seeded
            var accounts = Data.Models.Account.GetAll();
            Assert.IsTrue(accounts.Count > 0);
        }

        [TestMethod]
        public void TestMethodAddReading()
        {
            // Pick a random account
            List<Account> allAccounts = Account.GetAll();
            var random = new Random(DateTime.Now.Millisecond);
            var testAccount = allAccounts[random.Next(allAccounts.Count)].AccountId;

            // Create a new random meter reading for this account
            int meterReadingValue = random.Next(100000);
            DateTime readingTime = DateTime.UtcNow;
            var meterReading = new MeterReading(testAccount, readingTime, meterReadingValue);

            // Count the number of existing MeterReading records
            var existingCount = MeterReading.GetAll().Count;

            bool worked = meterReading.TryPersist();
            if (!worked)
            {
                Assert.Fail();
            }
            else
            {
                // Check whether the record was correctly added
                var newCount = MeterReading.GetAll().Count;
                Assert.IsTrue(newCount == existingCount + 1);

                // Delete record to tidy up database
                int recordsDeleted = meterReading.Delete();
                Assert.IsTrue(recordsDeleted == 1);

                // Check the record was correctly deleted
                Assert.IsTrue(existingCount == MeterReading.GetAll().Count);
            }
        }

        [TestMethod]
        public void TestMethodAddExistingReading()
        {
            // Get an existing reading and try to add it again
            var allReadings = MeterReading.GetAll();
            if (allReadings.Count > 0)
            {
                var random = new Random(DateTime.Now.Millisecond);
                var testReading = allReadings[random.Next(allReadings.Count)];
                var existingRecordCount = allReadings.Count;
                if (testReading.TryPersist())
                {
                    Assert.Fail();
                }
                else
                {
                    allReadings = MeterReading.GetAll();
                    if (allReadings.Count != existingRecordCount)
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethodAddOldReading()
        {
            var allReadings = MeterReading.GetAll();
            if (allReadings.Count > 0)
            {
                // Get a random existing reading
                var random = new Random(DateTime.Now.Millisecond);
                var testReading = allReadings[random.Next(allReadings.Count)];

                // Create one for the same account in the past
                // Try to persist it and verify that it could not be added
                if (testReading != null)
                {
                    var newTestReading = new RawMeterReading(testReading.AccountId.ToString(), testReading.MeterReadingDateTime.AddDays(-1).ToString(), testReading.MeterReadValue.ToString());
                    if (newTestReading.Valid())
                    {
                        Assert.Fail();
                    }
                }
            }
        }
    }
}