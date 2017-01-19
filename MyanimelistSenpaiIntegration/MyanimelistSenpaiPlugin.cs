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
        private bool updateInProgress = true;
        private DateTime LastUpdate = DateTime.Now;
        private String username_;
        private int updateDelay_;

        internal Measure(String username,int updateDelay)
        {
            try
            {
                if(Plugin.AnimeIntegration != null)
                {
                    Plugin.AnimeIntegration.RequestUserAnimelist(username);
                    Plugin.AnimeIntegration.RequestSenpai();
                }
            }
            catch (Exception)
            {

            }

            LastUpdate = DateTime.Now;

            username_ = username;
            updateDelay_ = updateDelay;
        }

        internal void Reload(Rainmeter.API rm, ref double maxValue)
        {
            //Plugin.AnimeIntegration.Reload();
            //List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();
            //List<Anime> ptw = Plugin.AnimeIntegration.GetLatestUpdates(5);

            //if(upcoming.Count > 0 && ptw.Count > 0 && initComplete)
            //{
            //    String username = rm.ReadString("MalUser", "");
            //    API.Log(API.LogType.Debug, "Reloading MyAnimelist for user " + username);

            //    Plugin.AnimeIntegration.Reload();
            //    Plugin.AnimeIntegration.RequestUserAnimelist(username);
            //    Plugin.AnimeIntegration.RequestSenpai();

            //    updateInProgress = true;
            //}
        }

        internal double Update()
        {
            if (!updateInProgress)
            {
                bool shouldUpdate = false;
                DateTime now = DateTime.Now;

                TimeSpan diff = now.Subtract(LastUpdate);

                if (diff.Minutes >= updateDelay_)
                {
                    shouldUpdate = true;
                    LastUpdate = now;

                    try
                    {
                        if (Plugin.AnimeIntegration != null)
                        {
                            Plugin.AnimeIntegration.Reload();
                            Plugin.AnimeIntegration.RequestUserAnimelist(username_);
                            Plugin.AnimeIntegration.RequestSenpai();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                API.Log(API.LogType.Debug, "Should update: " + shouldUpdate.ToString() + " Update Interval: " + updateDelay_.ToString());

                if (shouldUpdate)
                {
                    API.Log(API.LogType.Debug, "Updating...");
                }
            }
            return 0.0;
        }

        internal string GetString()
        {
            if (updateInProgress)
            {
                API.Log(API.LogType.Debug, "Waiting for MAL/Senpai requests to complete...");
            }

            List<SenpaiItem> upcoming = Plugin.AnimeIntegration.GetUpcomingAnime();
            List<Anime> ptw = Plugin.AnimeIntegration.GetLatestUpdates(5);

            PluginReturnJson returnValue = new PluginReturnJson();

            bool stage1 = false;
            bool stage2 = false;

            if (upcoming.Count == 0)
            {
                returnValue.UpcomingAnime.UpcomingRequestStatus = false;
            }
            else
            {
                returnValue.UpcomingAnime.UpcomingRequestStatus = true;
                returnValue.UpcomingAnime.UpcomingAnime = upcoming;

                stage1 = true;
            }

            if (ptw.Count == 0)
            {
                returnValue.LatestUpdates.LatestUpdatesRequestStatus = false;
            }
            else
            {
                returnValue.LatestUpdates.LatestUpdatesRequestStatus = true;
                returnValue.LatestUpdates.LatestUpdates = ptw;

                stage2 = true;
            }

            updateInProgress = !(stage1 && stage2);
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
            String updateDelay = api.ReadString("UpdateDelay", "");
            int iUpdateDelay = 60; // 1 hour

            try
            {
                iUpdateDelay = Int32.Parse(updateDelay);
            }
            catch (Exception)
            {
                
            }

            if (!String.IsNullOrEmpty(username) && AnimeIntegration == null)
            {
                AnimeIntegration = new Integration(username);
            }

            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure(username,iUpdateDelay)));
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
