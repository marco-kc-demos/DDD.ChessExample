using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDD.Core.Test
{
    [TestClass]
    public class AggregateTest
    {
        [TestMethod]
        public void AnAggregateIsAlwaysAnEntity()
        {
            long id = 1;
            Bank target = new Bank(id);

            Assert.IsInstanceOfType(target, typeof(Entity<long>));
        }

        [TestMethod]
        public void WhenEventIsRaisedItIsAddedToEvents()
        {
            Bank target = new Bank(1);
            target.OpenAccount(new OpenAccount("Jan"));

            var evt = target.Events.First();
            Assert.IsInstanceOfType(evt, typeof(AccountOpened));
            AccountOpened ao = (AccountOpened)evt;
            Assert.AreEqual("Jan", ao.Owner);
        }

        [TestMethod]
        public void TwoEventsAreAddedToEvents()
        {
            Bank target = new Bank(1);
            target.OpenAccount(new OpenAccount("Jan"));
            target.OpenAccount(new OpenAccount("Fatima"));

            Assert.AreEqual(2, target.Events.Count());
            var evts = target.Events.Cast<AccountOpened>();
            Assert.IsTrue(evts.Any(e => e.Owner == "Jan"));
            Assert.IsTrue(evts.Any(e => e.Owner == "Fatima"));
        }

        [TestMethod]
        public void ClearEventsClearsTheEventList()
        {
            Bank target = new Bank(1);
            target.OpenAccount(new OpenAccount("Jan"));
            target.OpenAccount(new OpenAccount("Fatima"));

            target.ClearEvents();

            Assert.IsFalse(target.Events.Any());
        }

        [TestMethod]
        public void NewAggregateHasVersion_0()
        {
            Bank target = new Bank(1);

            Assert.AreEqual(0, target.Version);
        }

        [TestMethod]
        public void EachEventIncreasesTheVersion()
        {
            Bank target = new Bank(1);
            target.OpenAccount(new OpenAccount("Jan"));
            target.OpenAccount(new OpenAccount("Fatima"));

            Assert.AreEqual(2, target.Version);
        }

        [TestMethod]
        public void WhenMethodCallsHandleMethod()
        {
            Bank target = new Bank(1);

            target.OpenAccount(new OpenAccount("Jan"));

            Assert.AreEqual(1, target.HandleAccountOpenedCallCount);
            Assert.IsTrue(target.HandleAccountOpenedArgument.Owner == "Jan");
            Assert.AreEqual(false, target.HandleAccountIsReplaying);
        }

        [TestMethod]
        public void TheConstructorReplaysEvents()
        {
            AccountOpened[] events =
            {
                new AccountOpened(1,"Jan"),
                new AccountOpened(2,"Fatima"),
            };

            Bank target = new Bank(1, events);

            Assert.AreEqual(2, target.HandleAccountOpenedCallCount);
            Assert.AreEqual(true, target.HandleAccountIsReplaying);
        }

        [TestMethod]
        public void ReplayedEventsDoNotAppearInEventList()
        {
            AccountOpened[] events =
            {
                new AccountOpened(1,"Jan"),
                new AccountOpened(2,"Fatima"),
            };

            Bank target = new Bank(1, events);

            Assert.AreEqual(0, target.Events.Count());
        }

        [TestMethod]
        public void NewAggregateHasOriginalVersion_0()
        {
            Bank target = new Bank(1);

            Assert.AreEqual(0, target.OriginalVersion);
        }

        [TestMethod]
        public void EventsDoNotIncreaseTheOriginalVersion()
        {
            Bank target = new Bank(1);
            target.OpenAccount(new OpenAccount("Jan"));
            target.OpenAccount(new OpenAccount("Fatima"));

            Assert.AreEqual(0, target.OriginalVersion);
        }

        [TestMethod]
        public void WhenReplayingEventsBothVersionAndOriginalVersionAreIncreased()
        {
            AccountOpened[] events =
            {
                new AccountOpened(1,"Jan"),
                new AccountOpened(2,"Fatima"),
            };

            Bank target = new Bank(1, events);

            Assert.AreEqual(2, target.OriginalVersion);
            Assert.AreEqual(2, target.Version);
        }

    }
}
