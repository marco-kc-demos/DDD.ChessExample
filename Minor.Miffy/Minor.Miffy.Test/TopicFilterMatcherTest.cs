using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minor.Miffy.RabbitMQBus.Test
{
    [TestClass]
    public class TopicFilterMatcherTest
    {
        [TestMethod]
        public void IsMatchTest()
        {
            Assert.IsTrue(TopicFilterMatcher.IsMatch("MVM.Test.Match", "MVM.Test.Match"), "MVM.Test.Match == MVM.Test.Match");
            Assert.IsFalse(TopicFilterMatcher.IsMatch("MVM.Test.NoMatch", "MVM.Test.Match"), "MVM.Test.NoMatch  !=  MVM.Test.Match");

            Assert.IsTrue(TopicFilterMatcher.IsMatch("MVM.Test.*", "MVM.Test.Match"), "MVM.Test.* == MVM.Test.Match");
            Assert.IsTrue(TopicFilterMatcher.IsMatch("MVM.*.Match", "MVM.Test.Match"), "MVM.*.Match == MVM.Test.Match");
            Assert.IsTrue(TopicFilterMatcher.IsMatch("*.Test.Match", "MVM.Test.Match"), "*.Test.Match == MVM.Test.Match");
            Assert.IsFalse(TopicFilterMatcher.IsMatch("*.Match", "MVM.Test.Match"), "*.Match  !=  MVM.Test.Match");
            Assert.IsFalse(TopicFilterMatcher.IsMatch("MVM.*.Match", "MVM.Test.To.Match"), "MVM.*.Match  !=  MVM.Test.To.Match");

            Assert.IsTrue(TopicFilterMatcher.IsMatch("#.Match", "MVM.Test.Match"), "#.Match  ==  MVM.Test.Match");
            Assert.IsTrue(TopicFilterMatcher.IsMatch("#", "MVM.Test.Match"), "#  ==  MVM.Test.Match");
        }

        [TestMethod]
        public void IsValidTopicFilterTest()
        {
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("Test"), "'Test' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("*"), "'*' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("#"), "'#' should be a valid expression.");

            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.Test"), "'MVM.Test' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.*"), "'MVM.*' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.#"), "'MVM.#' should be a valid expression.");

            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("Test.Event"), "'Test.Event' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("*.Event"), "'*.Event' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("#.Event"), "'#.Event' should be a valid expression.");

            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.Test.Event"), "'MVM.Test.Event' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.*.Event"), "'MVM.*.Event' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("MVM.#.Event"), "'MVM.#.Event' should be a valid expression.");

            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("Test.Event.#"), "'Test.Event.#' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("*.Event.#"), "'*.Event.#' should be a valid expression.");
            Assert.IsTrue(TopicFilterMatcher.IsValidTopicFilter("*.*.#.Event.#"), "'*.*.#.Event.#' should be a valid expression.");

            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("#Event"), "'#Event' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("*Event"), "'*Event' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("*#"), "'*#' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("MVM.#Event"), "'MVM.#Event' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("MVM.*Event"), "'MVM.*Event' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter("MVM.*#"), "'MVM.*#' should NOT be a valid expression.");
            Assert.IsFalse(TopicFilterMatcher.IsValidTopicFilter(""), "[empty string] should NOT be a valid expression.");
        }

        [TestMethod]
        public void MatchShouldFindMultipleTopicFilters()
        {
            // Arrange
            string[] topicFilters = { "MVM.*.Event", "*.Test.Event", "*.NoTest.Event", "MVM.#" };
            string topic = "MVM.Test.Event";

            // Act
            IEnumerable<string> matchingFilters = topicFilters.ThatMatch(topic);

            // Assert
            var matchingList = matchingFilters.ToList();
            CollectionAssert.Contains(matchingList, "MVM.*.Event", "The expression 'MVM.*.Event' should match");
            CollectionAssert.Contains(matchingList, "*.Test.Event", "The expression '*.Test.Event' should match");
            CollectionAssert.Contains(matchingList, "MVM.#", "The expression 'MVM.#' should match");
            CollectionAssert.DoesNotContain(matchingList, "*.NoTest.Event", "The expression '*.NoTest.Event' should NOT match");

        }
    }
}
