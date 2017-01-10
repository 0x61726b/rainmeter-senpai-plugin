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

    public enum Status
    {
        Airing = 1,
        Finished = 2,
        NotYetAired = 3
    }

    public class Nyaa
    {
        public String Title { get; set; }
        public String Link { get; set; }
        public String Desc { get; set; }

        public Nyaa(XmlNode title,XmlNode link,XmlNode desc)
        {
            this.Title = title.InnerText;
            this.Link = link.InnerText;
            this.Desc = desc.InnerText;
        }
    }
    public class Anime
    {
        public int MalId { get; set; }
        public String Title { get; set; }
        public AnimeStatus UserStatus { get; set; }
        public Status Status { get; set; }
        public int LastUpdated { get; set; } //Unix timestamp
        public int WatchedEpisodes { get; set; }
        public int TotalEpisodes { get; set; }

        public Anime(XmlNode id,XmlNode title, XmlNode status, XmlNode lastUpdated,XmlNode animestatus,XmlNode watched,XmlNode total)
        {
            this.MalId = Int32.Parse(id.InnerText);
            this.Title = title.InnerText;
            this.WatchedEpisodes = Int32.Parse(watched.InnerText);
            this.TotalEpisodes = Int32.Parse(total.InnerText);
            

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
                default:
                    break;
            }

            switch(animestatus.InnerText)
            {
                case "1":
                    this.Status = Status.Airing;
                    break;

                case "2":
                    this.Status = Status.NotYetAired;
                    break;

                case "3":
                    this.Status = Status.Finished;
                    break;

                default:
                    break;
            }

            this.LastUpdated = Int32.Parse(lastUpdated.InnerText);
        }
    }
}
