using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AnimeServicesIntegration
{
    public enum AnimeStatus
    {
        Watching = 1,
        Completed = 2,
        OnHold = 3,
        Dropped = 4,
        PlanToWatch = 6
    }
    public class Anime
    {
        public int MalId { get; set; }
        public String Title { get; set; }
        public AnimeStatus UserStatus { get; set; }
        public int LastUpdated { get; set; } //Unix timestamp

        public Anime(XmlNode id,XmlNode title, XmlNode status, XmlNode lastUpdated)
        {
            this.MalId = Int32.Parse(id.InnerText);
            this.Title = title.InnerText;

            

            switch (status.InnerText)
            {
                case "1":
                    this.UserStatus = AnimeStatus.Watching;
                    break;
                case "2":
                    this.UserStatus = AnimeStatus.Completed;
                    break;
                case "3":
                    this.UserStatus = AnimeStatus.OnHold;
                    break;
                case "4":
                    this.UserStatus = AnimeStatus.Dropped;
                    break;
                case "6":
                    this.UserStatus = AnimeStatus.PlanToWatch;
                    break;
            }

            this.LastUpdated = Int32.Parse(lastUpdated.InnerText);
        }
    }
}
