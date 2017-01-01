using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;
using AnimeServicesIntegration;

namespace MALSenpaiPlugin
{
    internal class Measure
    {
        enum MeasureType
        {
            Baka,
            LatestPtw, //LUL
            LatestPtw2, //LUL
            LatestPtw3, //LUL
            LatestPtw4, //LUL
            LatestPtw5, //LUL
            Upcoming0Title, //LUL
            Upcoming1Title, //LUL
            Upcoming2Title, //LUL
            Upcoming3Title, //LUL
            Upcoming4Title, //LUL
            Upcoming0Day, //LUL
            Upcoming1Day, //LUL
            Upcoming2Day, //LUL
            Upcoming3Day, //LUL
            Upcoming4Day, //LUL
            Upcoming0Hour, //LUL
            Upcoming1Hour, //LUL
            Upcoming2Hour, //LUL
            Upcoming3Hour, //LUL
            Upcoming4Hour, //LUL
            Upcoming0Id, //LUL
            Upcoming1Id, //LUL
            Upcoming2Id, //LUL
            Upcoming3Id, //LUL
            Upcoming4Id //LUL

        }

        private MeasureType Type = MeasureType.Baka;

        internal Measure()
        {

        }

        internal void Reload(Rainmeter.API rm, ref double maxValue)
        {
            string type = rm.ReadString("Type", "");
            switch (type.ToLowerInvariant())
            {
                // Titles
                case "upcoming0title":
                    Type = MeasureType.Upcoming0Title;
                    break;

                case "upcoming1title":
                    Type = MeasureType.Upcoming1Title;
                    break;

                case "upcoming2title":
                    Type = MeasureType.Upcoming2Title;
                    break;

                case "upcoming3title":
                    Type = MeasureType.Upcoming3Title;
                    break;

                case "upcoming4title":
                    Type = MeasureType.Upcoming4Title;
                    break;

                // Days
                case "upcoming0day":
                    Type = MeasureType.Upcoming0Day;
                    break;

                case "upcoming1day":
                    Type = MeasureType.Upcoming1Day;
                    break;

                case "upcoming2day":
                    Type = MeasureType.Upcoming2Day;
                    break;

                case "upcoming3day":
                    Type = MeasureType.Upcoming3Day;
                    break;

                case "upcoming4day":
                    Type = MeasureType.Upcoming4Day;
                    break;

                // Hours
                case "upcoming0hour":
                    Type = MeasureType.Upcoming0Hour;
                    break;

                case "upcoming1hour":
                    Type = MeasureType.Upcoming1Hour;
                    break;

                case "upcoming2hour":
                    Type = MeasureType.Upcoming2Hour;
                    break;

                case "upcoming3hour":
                    Type = MeasureType.Upcoming3Hour;
                    break;

                case "upcoming4hour":
                    Type = MeasureType.Upcoming4Hour;
                    break;

                //
                case "latestptw":
                    Type = MeasureType.LatestPtw;
                    break;

                case "latestptw2":
                    Type = MeasureType.LatestPtw2;
                    break;

                case "latestptw3":
                    Type = MeasureType.LatestPtw3;
                    break;

                case "latestptw4":
                    Type = MeasureType.LatestPtw4;
                    break;

                case "latestptw5":
                    Type = MeasureType.LatestPtw5;
                    break;

                // Ids
                case "upcoming0id":
                    Type = MeasureType.Upcoming0Id;
                    break;

                case "upcoming1id":
                    Type = MeasureType.Upcoming1Id;
                    break;

                case "upcoming2id":
                    Type = MeasureType.Upcoming2Id;
                    break;

                case "upcoming3id":
                    Type = MeasureType.Upcoming3Id;
                    break;

                case "upcoming4id":
                    Type = MeasureType.Upcoming4Id;
                    break;

                default:
                    API.Log(API.LogType.Error, "SystemVersion.dll: Type=" + type + " not valid");
                    break;
            }
        }

        internal double Update()
        {
            return 0.0;
        }

        internal string GetString()
        {
            int type = (int)Type;

            if ((int)Type > 0 && (int)Type < 6) // LUL
            {
                List<Anime> ptw = Plugin.AnimeIntegration.GetLatestPlantoWatch(5);

                if (ptw.Count == 0)
                {
                    return "Fetching...";
                }

                // Le dirty hack
                if (ptw.Count > (int)Type - 1)
                {
                    return ptw[(int)Type - 1].Title;
                }
                else
                {
                    return "No entry.";
                }
            }

            if (type > 5 && type < 11) //LUL
            {
                List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();

                if (upcoming.Count == 0)
                {
                    return "Fetching...";
                }

                if (upcoming.Count > type - 6)
                {
                    return upcoming[type - 6].Name;
                }
            }

            if (type > 10 && type < 16)
            {
                List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();

                if (upcoming.Count == 0)
                {
                    return "Fetching...";
                }

                if (upcoming.Count > type - 11)
                {
                    return upcoming[type - 11].UserTimezone.rd_weekday;
                }
            }

            if (type > 15 && type < 21)
            {
                List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();

                if (upcoming.Count == 0)
                {
                    return "Fetching...";
                }

                if (upcoming.Count > type - 16)
                {
                    return upcoming[type - 16].UserTimezone.rd_time;
                }
            }

            if (type > 20 && type < 26)
            {
                List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();

                if (upcoming.Count == 0)
                {
                    return "0";
                }

                if (upcoming.Count > type - 21)
                {
                    return upcoming[type - 21].MalId.ToString();
                }
            }

            // MeasureType.Major, MeasureType.Minor, and MeasureType.Number are
            // numbers. Therefore, null is returned here for them. This is to
            // inform Rainmeter that it can treat those types as numbers.

            return null;
        }
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;
        public static Integration AnimeIntegration = null; // Important

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            Rainmeter.API api = new Rainmeter.API(rm);

            String username = api.ReadString("MalUser", "");

            if (!String.IsNullOrEmpty(username) && AnimeIntegration == null)
            {
                API.Log(API.LogType.Error, "Requesting MyAnimelist for user " + username);
                AnimeIntegration = new Integration(username);
                AnimeIntegration.RequestUserAnimelist(username);
                AnimeIntegration.RequestSenpai();
            }

            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
    }
}
