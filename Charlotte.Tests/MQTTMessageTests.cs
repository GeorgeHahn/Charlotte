using System;
using Xunit;

namespace Charlotte.Tests
{
    public class MqttMessageTests
    {
        [Fact]
        public void CanIndex()
        {
            dynamic msg = new MqttMessage();

            msg["anyvalue"] = "something";

            Assert.Equal("something", msg["anyvalue"]);
        }

        [Fact]
        public void CanAccessDirectly()
        {
            dynamic msg = new MqttMessage();

            msg.anyvalue = "something";

            Assert.Equal("something", msg.anyvalue);
        }

        [Fact]
        public void WillAcceptNonStrings()
        {
            dynamic msg = new MqttMessage();

            msg.anyvalue = 0;

            Assert.Equal("0", msg.anyvalue);
        }

        [Fact]
        public void InvalidNamesThrow()
        {
            dynamic msg = new MqttMessage();

            Assert.Throws<MissingMemberException>(() => { var test = msg.anyvalue; });
        }
    }
}
