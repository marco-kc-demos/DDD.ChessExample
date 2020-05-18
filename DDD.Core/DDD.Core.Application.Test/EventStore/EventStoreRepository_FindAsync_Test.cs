using DDD.Core.Application.EventStoreModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDD.Core.Application.Test
{
    [TestClass]
    public class EventStoreRepository_LoadAsync_Test
    {
        private SqliteConnection _connection;
        private DbContextOptions<BankEventStoreContext> _options;
        private DbContextOptions<Raw.RawContext> _rawOptions;
        #region Set up in-memory database
        [TestInitialize]
        public void TestInitialize()
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<BankEventStoreContext>()
                .UseSqlite(_connection)
                .Options;
            _rawOptions = new DbContextOptionsBuilder<Raw.RawContext>()
                .UseSqlite(_connection)
                .Options;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _connection.Close();
        }
        #endregion

        [TestMethod]
        public async Task LoadFromEmptyDatabaseYields_null()
        {
            var target = new BankRepository(_options);

            Bank bank = await target.FindAsync(1);

            Assert.IsNull(bank);
        }

        [TestMethod]
        public async Task LoadAggregateRootWithZeroEvents()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var bankje = new Raw.Bank { Id = 7, Version = 12, };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            var target = new BankRepository(_options);

            Bank bank = await target.FindAsync(7);

            Assert.AreEqual(7, bank.Id);
            Assert.AreEqual(0, bank.Version);   
            // the 'Version' should reflect the number of events that have been handled, which is 0, in this test.
            // this should be equal the the 'Version' in the database, but in this test the latter was manually set to 12.
        }

        [TestMethod]
        public async Task LoadAggregateRootWithOneEvent()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        Id = 1,
                        BankId = 7,
                        Version = 1,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{ 'AccountNumber':5, 'Owner':'Karina van Irak' }",
                    }
                };
                var bankje = new Raw.Bank { Id = 7, Version = 12, Events = events };

                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            var target = new BankRepository(_options);

            Bank bank = await target.FindAsync(7);

            Assert.AreEqual(7, bank.Id);
            Assert.AreEqual(1, bank.Version);
            Assert.AreEqual(1, bank.HandleAccountOpenedCallCount);
        }

        [TestMethod]
        public async Task LoadAggregateRootWithTwoEvents()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        Id = 1, BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{ 'AccountNumber':5, 'Owner':'Karina van Irak' }",
                    },
                    new Raw.BankEvent
                    {
                        Id = 2, BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{ 'AccountNumber':6, 'Owner':'Evert \\'t Reve' }",
                    },
                };
                var bankje = new Raw.Bank { Id = 7, Version = 12, Events = events };

                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            var target = new BankRepository(_options);

            Bank bank = await target.FindAsync(7);

            Assert.AreEqual(7, bank.Id);
            Assert.AreEqual(2, bank.Version);
            Assert.AreEqual(2, bank.HandleAccountOpenedCallCount);
            Assert.AreEqual(6, bank.HandleAccountOpenedArgument.AccountNumber);
            Assert.AreEqual("Evert 't Reve", bank.HandleAccountOpenedArgument.Owner);
        }



        [TestMethod]
        public async Task LoadAggregateRootWithOnyEventsFromOtherAggregates()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        Id = 1, BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{ 'AccountNumber':5, 'Owner':'Karina van Irak' }",
                    },
                    new Raw.BankEvent
                    {
                        Id = 2, BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{ 'AccountNumber':6, 'Owner':'Evert \\'t Reve' }",
                    },
                };
                rawContext.Banks.Add(new Raw.Bank { Id = 7, Version = 12, Events = events });
                rawContext.Banks.Add(new Raw.Bank { Id = 5, Version = 12, Events = new List<Raw.BankEvent>() });
                rawContext.Banks.Add(new Raw.Bank { Id = 3, Version = 12, Events = null });
                rawContext.SaveChanges();
            }

            var target = new BankRepository(_options);

            Bank bank = await target.FindAsync(5);

            Assert.AreEqual(5, bank.Id);
            Assert.AreEqual(0, bank.Version);
            Assert.AreEqual(0, bank.HandleAccountOpenedCallCount);
        }

        [TestMethod]
        public async Task LoadAggregaterootWhenThereIsNoDatabaseConnection()
        {
            var otheroptions = new DbContextOptionsBuilder<BankEventStoreContext>()
                .Options;
            var target = new BankRepository(otheroptions);

            Func<Task> act = () =>
            {
                return target.FindAsync(5);
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(act);
        }
    }
}
