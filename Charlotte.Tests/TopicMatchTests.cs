using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Should;

namespace Charlotte.Tests
{
    public class TopicMatchTests
    {
        [Fact]
        public void TopicsBoil()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.BoilWildcards("{wildcard}/{another}/{again}")
                .ShouldBe("+/+/+");
        }

        [Fact]
        public void MixedTopicsBoil()
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            matcher.BoilWildcards("{wildcard}/not_a_wildcard/{but_this_is}")
                .ShouldBe("+/not_a_wildcard/+");
        }

        [Fact]
        public void ThrowOnInvalidWildcards() // TODO
        {
            MqttTopicMatcher matcher = new MqttTopicMatcher();

            Assert.Throws<Exception>(() => { matcher.BoilWildcards("{wildcard}/not_a_wildcard/{this is invalid}"); });
        }
    }
}
