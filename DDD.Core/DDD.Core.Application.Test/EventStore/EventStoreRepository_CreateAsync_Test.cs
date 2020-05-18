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
    public class EventStoreRepository_CreateAsync_Test
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
        public async Task CreateAggregate()
        {
            var target = new BankRepository(_options);

            // Act
            Bank bank = await target.CreateAsync();

            // Assert
            using (var rawContext = new Raw.RawContext(_rawOptions))
            {
                var bankFromDb = rawContext.Banks.Include(b => b.Events).FirstOrDefault();

                Assert.IsNotNull(bankFromDb);
                Assert.AreEqual(1, bankFromDb.Id);
                Assert.AreEqual(0, bankFromDb.Version);
                Assert.IsFalse(bankFromDb.Events.Any());
            }
        }

    }
}
