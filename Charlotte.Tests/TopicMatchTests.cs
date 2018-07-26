using System.Linq;
using Should;
using Xunit;

namespace Charlotte.Tests
{
    public class TopicBoiler
    {
        [Fact]
        public void TopicsBoil()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.ConvertMatchingGroupsToMQTTWildcards("{wildcard}/{another}/{again}")
                .ShouldEqual("+/+/+");
        }

        [Fact]
        public void MixedTopicsBoil()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.ConvertMatchingGroupsToMQTTWildcards("{wildcard}/not_a_wildcard/{but_this_is}")
                .ShouldEqual("+/not_a_wildcard/+");
        }
    }

    public class TopicMatcher
    {
        [Fact]
        public void BasicTopicsMatch()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.TopicsMatch(null, "just/a/topic", "just/a/topic")
                .ShouldBeTrue();
        }

        [Fact]
        public void SingleLevelWildcardsMatch()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.TopicsMatch(new MqttMessage(), "{wildcard}/not_a_wildcard/{but_this_is}", "anything/not_a_wildcard/more_anything")
                .ShouldBeTrue();
        }

        [Fact]
        public void MultiLevelWildcardsMatch()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.TopicsMatch(null, "#", "this/should/match/everything")
                .ShouldBeTrue();
        }
    }

    public class WildCardExtraction
    {
        [Fact]
        public void ExtractsWildcards()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            var wildcards = matcher.ExtractWildcards("{thisis}/{a}/{bunch}/{ofwildcards}").ToArray();

            wildcards.ShouldContain("thisis");
            wildcards.ShouldContain("a");
            wildcards.ShouldContain("bunch");
            wildcards.ShouldContain("ofwildcards");
            wildcards.Count().ShouldEqual(4);
        }
    }

    public class WildcardIdentifiers
    {
        [Fact]
        public void ThrowOnInvalidWildcards()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            Assert.Throws<MqttTopicMatcher.InvalidWildcardException>(() => { matcher.VerifyWildcardNames("{wildcard}/not_a_wildcard/{#invalid}"); });
        }

        [Fact]
        public void DoesntThrowOnValidWildcards()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.VerifyWildcardNames("{wildcard}/not_a_wildcard/{this_is_valid}");
        }
    }
}
