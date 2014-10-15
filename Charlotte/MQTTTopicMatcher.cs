using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charlotte
{
    public class MqttTopicMatcher
    {
        public string BoilWildcards(string topic)
        {
            if (!(topic.Contains('{') && topic.Contains('}')))
                return topic;

            var str = BoilWildcards(topic.Replace(topic.Substring(topic.IndexOf('{'), topic.IndexOf('}') - topic.IndexOf('{') + 1), "+"));
            return str;
        }

        public bool TopicsMatch(dynamic message, string key, string topic)
        {
            if (key == topic)
                return true;

            if (key.Contains('{') && key.Contains('}'))
            {
                string wildcardName = key.Substring(key.IndexOf('{') + 1, key.IndexOf('}') - key.IndexOf('{') - 1);
                if (TopicsMatch(message, key.Replace('{' + wildcardName + '}', "+"), topic))
                {
                    int wildcardpos = key.IndexOf(wildcardName) - 1;
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
                        actualname = topic.Substring(0, topic.IndexOf('/'));
                    else
                        actualname = topic;

                    message[wildcardName] = actualname;
                    return true;
                }
            }

            if (!key.Contains('#') && !key.Contains('+'))
            {
                if (key != topic)
                    return false;
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
                    return false;

                for (int i = 0; i < keyparts.Length; i++)
                {
                    if (keyparts[i] == "+")
                        continue;

                    if (keyparts[i] != topicparts[i])
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
