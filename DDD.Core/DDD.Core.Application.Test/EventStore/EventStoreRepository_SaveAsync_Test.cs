using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Core.Application.Test
{
    [TestClass]
    public class EventStoreRepository_SaveAsync_Test
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
        public async Task SaveAggregateWithoutEvents()
        {
            Bank bank = new Bank(42);
            var target = new BankRepository(_options);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(42, bankFromDb.Id);
                Assert.AreEqual(0, bankFromDb.Version);
                Assert.IsFalse(bankFromDb.Events.Any());
            }
        }

        [TestMethod]
        public async Task SaveAggregateWithTwoEvents()
        {
            Bank bank = new Bank(43);
            bank.OpenAccount(new OpenAccount("Karina van Irak"));
            bank.OpenAccount(new OpenAccount("Evert 't Reve"));
            Assert.AreEqual(2, bank.Events.Count());
            var target = new BankRepository(_options);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b=>b.Events).Single(b => b.Id == 43);

                Assert.AreEqual(2, bankFromDb.Version);
                Assert.AreEqual(2, bankFromDb.Events.Count());
                Assert.IsTrue(bankFromDb.Events.Any(evt => 
                    evt.EventData.Contains("Karina van Irak")));
                Assert.IsTrue(bankFromDb.Events.Any(evt => 
                    evt.EventData.Contains("Evert 't Reve")));
            }
        }

        [TestMethod]
        public async Task SaveAggregateWithSerializedEvent()
        {
            Bank bank = new Bank(43);
            bank.OpenAccount(new OpenAccount("Evert 't Reve"));
            var target = new BankRepository(_options);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var onlyEvent = rawContext.BankEvents.Single();

                Assert.AreEqual(1, onlyEvent.Version);
                Assert.AreEqual("DDD.Core.Application.Test.AccountOpened", onlyEvent.EventType);
                Assert.AreEqual("{\r\n  \"AccountNumber\": 1,\r\n  \"Owner\": \"Evert 't Reve\"\r\n}", 
                    onlyEvent.EventData);
            }
        }

        [TestMethod]
        public async Task SaveAggregate_AlreadyInStoreWithoutEvents()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var bankje = new Raw.Bank { Id = 7, Version = 0, };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            Bank bank = new Bank(7);
            var target = new BankRepository(_options);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(7, bankFromDb.Id);
                Assert.AreEqual(0, bankFromDb.Version);
                Assert.IsFalse(bankFromDb.Events.Any());
            }
        }

        [TestMethod]
        public async Task SaveAggregate_AlreadyInStoreWithoutEvents_AddingOne()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var bankje = new Raw.Bank { Id = 17, Version = 0, };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            Bank bank = new Bank(17);
            bank.OpenAccount(new OpenAccount("Mats Nevenstam"));
            var target = new BankRepository(_options);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(17, bankFromDb.Id);
                Assert.AreEqual(1, bankFromDb.Version);

                var onlyEvent = rawContext.BankEvents.Single();
                Assert.AreEqual(1, onlyEvent.Version);
                Assert.AreEqual("DDD.Core.Application.Test.AccountOpened", onlyEvent.EventType);
                Assert.IsTrue(onlyEvent.EventData.Contains("Mats Nevenstam"));
            }
        }

        [TestMethod]
        public async Task SaveAggregate_AlreadyInStoreWithEvents()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{\r\n  \"AccountNumber\": 5,\r\n  \"Owner\": \"Karina van Irak\"\r\n}",
                    },
                    new Raw.BankEvent
                    {
                        BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{\r\n  \"AccountNumber\": 6,\r\n  \"Owner\": \"Evert 't Reve\"\r\n}",
                    },
                };
                var bankje = new Raw.Bank { Id = 7, Version = 2, Events = events };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();

            }

            var target = new BankRepository(_options);
            Bank bank = await target.FindAsync(7);

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(7, bankFromDb.Id);
                Assert.AreEqual(2, bankFromDb.Version);
                Assert.AreEqual(2, bankFromDb.Events.Count());
            }
        }

        [TestMethod]
        public async Task SaveAggregate_AlreadyInStoreWithEvents_AddingThree()
        {
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{\r\n  \"AccountNumber\": 5,\r\n  \"Owner\": \"Karina van Irak\"\r\n}",
                    },
                    new Raw.BankEvent
                    {
                        BankId = 7, Version = 2,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{\r\n  \"AccountNumber\": 6,\r\n  \"Owner\": \"Evert 't Reve\"\r\n}",
                    },
                };
                var bankje = new Raw.Bank { Id = 7, Version = 2, Events = events };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();

            }

            var target = new BankRepository(_options);
            Bank bank = await target.FindAsync(7);
            bank.OpenAccount(new OpenAccount("Karel Lillerak"));
            bank.OpenAccount(new OpenAccount("Sara van Avaras"));
            bank.OpenAccount(new OpenAccount("Mats Nevenstam"));

            await target.SaveAsync(bank);

            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(7, bankFromDb.Id);
                Assert.AreEqual(5, bankFromDb.Version);
                List<Raw.BankEvent> eventList = bankFromDb.Events.ToList();
                Assert.AreEqual(5, eventList.Count);
                Assert.IsTrue(eventList[0].EventData.Contains("Karina van Irak"));
                Assert.IsTrue(eventList[1].EventData.Contains("Evert 't Reve"));
                Assert.IsTrue(eventList[2].EventData.Contains("Karel Lillerak"));
                Assert.IsTrue(eventList[3].EventData.Contains("Sara van Avaras"));
                Assert.IsTrue(eventList[4].EventData.Contains("Mats Nevenstam"));
            }
        }


        [TestMethod]
        public async Task SaveAggregate_WhenPersistentStoreIsModifiedSinceRead()
        {
            // setup database: bank with 1 event
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                rawContext.Database.EnsureCreated();

                var events = new List<Raw.BankEvent> {
                    new Raw.BankEvent
                    {
                        BankId = 7, Version = 1,
                        EventType = "DDD.Core.Application.Test.AccountOpened",
                        EventData = "{\r\n  \"AccountNumber\": 5,\r\n  \"Owner\": \"Karina van Irak\"\r\n}",
                    },
                };
                var bankje = new Raw.Bank { Id = 7, Version = 1, Events = events };
                rawContext.Banks.Add(bankje);
                rawContext.SaveChanges();
            }

            // load bank from database
            var target = new BankRepository(_options);
            Bank bank = await target.FindAsync(7);

            // modifiy database by adding an extra event
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankje = rawContext.Banks.Include(b => b.Events).Single(b => b.Id==7);
                bankje.Version = 2;
                bankje.Events.Add(new Raw.BankEvent
                {
                    BankId = 7,
                    Version = 2,
                    EventType = "DDD.Core.Application.Test.AccountOpened",
                    EventData = "{\r\n  \"AccountNumber\": 6,\r\n  \"Owner\": \"Evert 't Reve\"\r\n}",
                });
                rawContext.SaveChanges();
            }

            Func<Task> act = () =>
            {
                return target.SaveAsync(bank);
            };

            var ex = await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(act);
            Assert.IsTrue(ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s)."));
        }
    }
}
