using System;
using System.Collections.Generic;
using System.Text;

namespace MALSenpaiPlugin
{
    public class ItemTimezone
    {
        public String rd_date { get; set; }
        public String rd_time { get; set; }
        public int weekday { get; set; }
        public String rd_weekday { get; set; }
        public int weekday_sort { get; set; }
    }
    public class SenpaiItem
    {
        public String Name { get; set; }
        public int MalId { get; set; }
        public ItemTimezone UserTimezone { get; set; }
    }
}
