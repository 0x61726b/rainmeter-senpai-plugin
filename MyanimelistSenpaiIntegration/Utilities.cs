using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MALSenpaiPlugin
{
    public static class Utilities
    {
        public static double GetUserUtcOffsetInSeconds()
        {
            return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds;
        }

        public static List<SenpaiItem> ParseSenpai(String json)
        {
            JObject senpaiData = JObject.Parse(json);

            JToken meta = senpaiData["meta"];

            JEnumerable<JToken> items = senpaiData["items"].Children();

            String utcoffsetStr = GetUserUtcOffsetInSeconds().ToString();

            List<SenpaiItem> results = new List<SenpaiItem>();
            foreach(JToken token in items)
            { 
                JToken airdates = (token["airdates"]);

                ItemTimezone timezone = JsonConvert.DeserializeObject<ItemTimezone>(airdates[utcoffsetStr].ToString());

                SenpaiItem item = JsonConvert.DeserializeObject<SenpaiItem>(token.ToString());
                item.UserTimezone = timezone;
                results.Add(item);
            }
            return results;
        }
    }
}
