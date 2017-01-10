using MALSenpaiPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AnimeServicesIntegration
{

    public class Integration
    {
        private String userName_;
        private List<Anime> userList_;
        private List<SenpaiItem> senpaiData_;
        private List<Nyaa> torrents_;

        public Integration(String username)
        {
            this.userName_ = username;

            userList_ = new List<Anime>();
            senpaiData_ = new List<SenpaiItem>();
            torrents_ = new List<Nyaa>();
        }

        // Should not be called before ::RequestUserAnimelist
        public void RequestSenpai()
        {
            // To do, make it so that this method gets season name as parameter, which should come from Rainmeter
            String season = "winter2017";
            String url = "http://www.senpai.moe/export.php?type=json&src=" + season;

            AsyncRequest request = new AsyncRequest(new Uri(url), delegate (string results)
            {
                senpaiData_ = Utilities.ParseSenpai(results);
            });
        }

        public List<SenpaiItem> GetUpcomingAnime()
        {
            List<SenpaiItem> baka = new List<SenpaiItem>();
            List<SenpaiItem> results = new List<SenpaiItem>();

            if (senpaiData_.Count > 0)
            {
                //Compare senpai data with user list

                foreach (SenpaiItem item in senpaiData_)
                {
                    Anime anime = userList_.Find(x => x.MalId == item.MalId);

                    if (anime != null)
                    {
                        if (anime.UserStatus == AnimeStatus.Watching || anime.UserStatus == AnimeStatus.PlanToWatch)
                        {
                            baka.Add(item);
                        }
                    }

                }

                // Sort
                baka.Sort(delegate (SenpaiItem a, SenpaiItem b)
                {
                    return a.UserTimezone.weekday_sort.CompareTo(b.UserTimezone.weekday_sort);
                });

                // :thinking:
                int today = (int)DateTime.Now.DayOfWeek;

                // Senpai's sunday
                if (today == 0)
                {
                    today = 7;
                }


                //Upcoming
                for (int i = today; i <= 7; i++)
                {
                    foreach (SenpaiItem item in baka)
                    {
                        if (item.UserTimezone.weekday >= i)
                        {
                            // Check if we added already
                            if (!results.Contains(item))
                                results.Add(item);
                        }
                    }
                }

                if (results.Count < 4)
                {
                    //...downcoming ?
                    for (int i = 1; i < today; i++)
                    {
                        foreach (SenpaiItem item in baka)
                        {
                            if (item.UserTimezone.weekday == i)
                            {
                                results.Add(item);
                            }
                        }
                    }
                }
            }

            return results;
        }

        // Should not be called before ::RequestUserAnimelist
        public List<Anime> GetLatestPlantoWatchInternal()
        {
            List<Anime> list = new List<Anime>();

            if (userList_.Count > 0)
            {
                //Logic: Get all PTW entries, sort by lastUpdated, return first 10

                foreach (Anime entry in userList_)
                {
                    if (entry.UserStatus == AnimeStatus.PlanToWatch)
                    {
                        list.Add(entry);
                    }
                }

                //Sort
                list.Sort(delegate (Anime a, Anime b)
                {
                    return b.LastUpdated.CompareTo(a.LastUpdated);
                });
            }

            return list;
        }

        public List<Anime> GetLatestPlantoWatch(int limit)
        {
            List<Anime> list = GetLatestPlantoWatchInternal();
            if (list.Count > 0)
            {
                return list.GetRange(0, limit);
            }
            return list;
        }

        public List<Anime> GetLatestUpdatesInternal()
        {
            List<Anime> list = new List<Anime>();

            if (userList_.Count > 0)
            {
                list.AddRange(userList_);

                list.Sort(delegate (Anime a, Anime b)
                {
                    return b.LastUpdated.CompareTo(a.LastUpdated);
                });
            }
            return list;
        }

        public List<Anime> GetLatestUpdates(int limit)
        {
            List<Anime> list = GetLatestUpdatesInternal();
            if (list.Count > 0)
            {
                return list.GetRange(0, limit);
            }
            return list;
        }

        public void RequestTorrents()
        {
            String url = "https://www.nyaa.se/?page=rss&cats=1_37&filter=2";
            Uri uri = new Uri(url);

            AsyncRequest request = new AsyncRequest(uri, delegate (string results)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(results);

                XmlNodeList list = doc.SelectNodes("rss/channel/item");

                foreach(XmlNode element in list)
                {
                    XmlNode title = element.SelectSingleNode("title");
                    XmlNode link = element.SelectSingleNode("link");
                    XmlNode desc = element.SelectSingleNode("description");

                    Nyaa nyaa = new Nyaa(title, link, desc);
                    torrents_.Add(nyaa);
                }
            });
        }

        public void RequestUserAnimelist(String username)
        {
            String url = "http://myanimelist.net/malappinfo.php?u=" + username + "&type=anime&status=all";

            Uri uri = new Uri(url);

            if (userList_.Count > 0)
            {
                userList_.Clear();
            }

            AsyncRequest request = new AsyncRequest(uri, delegate (string results)
             {
                 XmlDocument doc = new XmlDocument();
                 doc.LoadXml(results);

                 XmlNodeList list = doc.SelectNodes("myanimelist/anime");

                 foreach (XmlNode element in list)
                 {
                     XmlNode id = element.SelectSingleNode("series_animedb_id");
                     XmlNode title = element.SelectSingleNode("series_title");
                     XmlNode status = element.SelectSingleNode("my_status");
                     XmlNode lastUpdated = element.SelectSingleNode("my_last_updated");
                     XmlNode animestatus = element.SelectSingleNode("series_status");

                     XmlNode watched = element.SelectSingleNode("my_watched_episodes");
                     XmlNode total = element.SelectSingleNode("series_episodes");
                     userList_.Add(new Anime(id, title, status, lastUpdated, animestatus, watched, total));
                 }

                 // TO do, make it so that Rainmeter calls update every 0.1 seconds while this is processing.
             });
        }

        public List<Anime> GetUserAnimelist()
        {
            return userList_;
        }
    }
}
