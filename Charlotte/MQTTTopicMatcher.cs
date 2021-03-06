using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Charlotte
{
    public class MqttTopicMatcher
    {
        public class InvalidWildcardException : Exception
        {
            public InvalidWildcardException(string message)
                : base(message)
            { }
        }

        public void VerifyWildcardNames(string topic)
        {
            // Don't allow some confusing wildcard names
            var ident = new Regex(@"([\{|\}|#|\+]+)");

            foreach (var wildcard in ExtractWildcards(topic))
            {
                var match = ident.Match(wildcard.Normalize());
                if (match.Success)
                {
                    throw new InvalidWildcardException($"Invalid wildcards in topic: {topic}, '{match.Groups[0].ToString()}'");
                }
            }
        }

        public IEnumerable<string> ExtractWildcards(string topic)
        {
            while (topic.Contains('{') && topic.Contains('}'))
            {
                string wildcardName = topic.Substring(topic.IndexOf('{') + 1, topic.IndexOf('}') - topic.IndexOf('{') - 1);
                topic = topic.Replace('{' + wildcardName + '}', "+");

                yield return wildcardName;
            }
        }

        // Todo: convert this from recursive -> iterative
        public string ConvertMatchingGroupsToMQTTWildcards(string topic)
        {
            // If the topic doesn't contain any matching portions, don't process it
            if (!(topic.Contains('{') && topic.Contains('}')))
            {
                return topic;
            }

            // Recursively replace matching portions with '+' wildcard
            var str = ConvertMatchingGroupsToMQTTWildcards(topic.Replace(topic.Substring(topic.IndexOf('{'), topic.IndexOf('}') - topic.IndexOf('{') + 1), "+"));
            return str;
        }

        // TODO: Match topics with a reasonable algorithm
        public bool TopicsMatch(dynamic message, string key, string topic)
        {
            if (key == topic)
            {
                return true;
            }

            if (key.Contains('{') && key.Contains('}'))
            {
                string wildcardName = key.Substring(key.IndexOf('{') + 1, key.IndexOf('}') - key.IndexOf('{') - 1);
                if (TopicsMatch(message, key.Replace('{' + wildcardName + '}', "+"), topic))
                {
                    int wildcardpos = key.IndexOf('{' + wildcardName + '}');
                    string wildcardx = key.Substring(0, wildcardpos);
                    int slashcount = 0;
                    while (wildcardx.Contains('/'))
                    {
                        wildcardx = wildcardx.Substring(wildcardx.IndexOf('/') + 1);
                        slashcount++;
                    }

                    while (slashcount > 0)
                    {
                        topic = topic.Substring(topic.IndexOf('/') + 1);
                        slashcount--;
                    }

                    string actualname;
                    if (topic.Contains('/'))
                    {
                        actualname = topic.Substring(0, topic.IndexOf('/'));
                    }
                    else
                    {
                        actualname = topic;
                    }

                    message[wildcardName] = actualname;
                    return true;
                }
            }

            if (!key.Contains('#') && !key.Contains('+'))
            {
                if (key != topic)
                {
                    return false;
                }
            }

            if (key.Contains('#'))
            {
                string keyfront = key.Substring(0, key.IndexOf('#'));
                string topicfront = topic.Substring(0, key.IndexOf('#'));
                return TopicsMatch(message, keyfront, topicfront);
            }

            if (key.Contains('+'))
            {
                string[] keyparts = key.Split('/');
                string[] topicparts = topic.Split('/');

                if (keyparts.Length != topicparts.Length)
                {
                    return false;
                }

                for (int i = 0; i < keyparts.Length; i++)
                {
                    if (keyparts[i] == "+")
                    {
                        continue;
                    }

                    if (keyparts[i] != topicparts[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}