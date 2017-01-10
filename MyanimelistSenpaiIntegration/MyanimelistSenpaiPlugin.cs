using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;
using AnimeServicesIntegration;
using Newtonsoft.Json;

namespace MALSenpaiPlugin
{
    public class ReturnUpcomingAnime
    {
        public bool UpcomingRequestStatus { get; set; }
        public List<SenpaiItem> UpcomingAnime { get; set; }
    }

    public class ReturnLatestUpdates
    {
        public bool LatestUpdatesRequestStatus { get; set; }
        public List<Anime> LatestUpdates { get; set; }
    }

    public class PluginReturnJson
    {
        public ReturnUpcomingAnime UpcomingAnime { get; set; }
        public ReturnLatestUpdates LatestUpdates { get; set; }

        public PluginReturnJson()
        {
            UpcomingAnime = new ReturnUpcomingAnime();
            LatestUpdates = new ReturnLatestUpdates();
        }
    }
    internal class Measure
    {

        internal Measure()
        {

        }

        internal void Reload(Rainmeter.API rm, ref double maxValue)
        {
            
        }

        internal double Update()
        {
            API.Log(API.LogType.Debug, "Waiting for MAL/Senpai requests to complete...");
            return 0.0;
        }

        internal string GetString()
        {
            List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();
            List<Anime> ptw = Plugin.AnimeIntegration.GetLatestUpdates(5);

            PluginReturnJson returnValue = new PluginReturnJson();

            if (upcoming.Count == 0)
            {
                returnValue.UpcomingAnime.UpcomingRequestStatus = false;
            }
            else
            {
                returnValue.UpcomingAnime.UpcomingRequestStatus = true;
                returnValue.UpcomingAnime.UpcomingAnime = upcoming;
            }

            if (ptw.Count == 0)
            {
                returnValue.LatestUpdates.LatestUpdatesRequestStatus = false;
            }
            else
            {
                returnValue.LatestUpdates.LatestUpdatesRequestStatus = true;
                returnValue.LatestUpdates.LatestUpdates = ptw;
            }
            return JsonConvert.SerializeObject(returnValue);
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
                AnimeIntegration = new Integration(username);
                API.Log(API.LogType.Debug, "Requesting MyAnimelist for user " + username);
                Plugin.AnimeIntegration.RequestUserAnimelist(username);
                Plugin.AnimeIntegration.RequestSenpai();
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
