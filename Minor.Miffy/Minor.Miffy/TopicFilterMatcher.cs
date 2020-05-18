using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Minor.Miffy
{
    /// <summary>
    /// Utility class for matching routingkeys to topic expressions.
    /// </summary>
    public static class TopicFilterMatcher
    {
        public static IEnumerable<string> ThatMatch(this IEnumerable<string> topicFilters, string topic)
        {
            return topicFilters.Where(expr => IsMatch(expr, topic));
        }

        public static bool IsMatch(string topicFilter, string topic)
        {
            var pattern = topicFilter
                      .Replace(@".", @"\.")
                      .Replace(@"*", @"[^.]*")
                      .Replace(@"#", @".*")
                      ;
            pattern = "^" + pattern + "$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(topic);
        }

        private const string part = @"([^.#*]+|\#|\*)";
        private const string TopicPattern = "^" + part + @"(\." + part + ")*$";
        private static readonly Regex ValidTopicRegex = new Regex(TopicPattern, RegexOptions.Compiled);

        public static bool IsValidTopicFilter(string topicFilter)
        {
            return ValidTopicRegex.IsMatch(topicFilter);
        }
    }
}