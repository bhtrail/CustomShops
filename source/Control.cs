using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BattleTech;
using HBS.Logging;
using HBS.Util;
using CustomShops.Shops;

namespace CustomShops
{
    public delegate void RefreshDelegate();

    public static class Control
    {
        private const string ModName = "CustomShops";
        private static string LogPrefix = "[CShops] ";
        public static CustomShopsSettings Settings = new CustomShopsSettings();

        public static GameState State { get; private set; }

        private static ILog Logger;
        private static FileLogAppender logAppender;

        internal static List<IShopDescriptor> Shops = new List<IShopDescriptor>();

        internal static List<IShopDescriptor> OnSystemChange = new List<IShopDescriptor>();
        internal static List<IShopDescriptor> OnMonthChange = new List<IShopDescriptor>();
        internal static List<IShopDescriptor> OnOwnerChange = new List<IShopDescriptor>();

        public static void Init(string directory, string settingsJSON)
        {
            Logger = HBS.Logging.Logger.GetLogger(ModName, LogLevel.Debug);

            try
            {
                try
                {
                    Settings = new CustomShopsSettings();
                    JSONSerializationUtility.FromJSON(Settings, settingsJSON);
                    HBS.Logging.Logger.SetLoggerLevel(Logger.Name, Settings.LogLevel);
                }
                catch (Exception)
                {
                    Settings = new CustomShopsSettings();
                }

                if (!Settings.AddLogPrefix)
                    LogPrefix = "";

                SetupLogging(directory);

                var harmony = HarmonyInstance.Create($"{ModName}");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.Log("=========================================================");
                Logger.Log($"Loaded {ModName} v0.1 for bt 1.9");
                Logger.Log("=========================================================");
                Logger.LogDebug("done");
                Logger.LogDebug(JSONSerializationUtility.ToJSON(Settings));

                State = new GameState();
                if (Settings.SystemShop)
                    RegisterShop(new SystemShop());
                if (Settings.FactionShop)
                    RegisterShop(new FactionShop());
                if (Settings.BlackMarketShop)
                    RegisterShop(new BlackMarketShop());


            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        public static void RegisterShop(IShopDescriptor shop)
        {
            if (shop != null)
            {
                Log($"Shop [{shop.Name}] registred");
                Shops.Add(shop);
                if (shop.RefreshOnMonthChange)
                    OnMonthChange.Add(shop);
                if (shop.RefreshOnOwnerChange)
                    OnOwnerChange.Add(shop);
                if (shop.RefreshOnSystemChange)
                    OnSystemChange.Add(shop);
            }
        }

        #region LOGGING
        [Conditional("CCDEBUG")]
        public static void LogDebug(DInfo type, string message)
        {
            if (Settings.DebugType.HasFlag(type))
                Logger.LogDebug(LogPrefix + message);
        }
        [Conditional("CCDEBUG")]
        public static void LogDebug(DInfo type, string message, Exception e)
        {
            if (Settings.DebugType.HasFlag(type))
                Logger.LogDebug(LogPrefix + message, e);
        }

        public static void LogError(string message)
        {
            Logger.LogError(LogPrefix + message);
        }
        public static void LogError(string message, Exception e)
        {
            Logger.LogError(LogPrefix + message, e);
        }
        public static void LogError(Exception e)
        {
            Logger.LogError(LogPrefix, e);
        }

        public static void Log(string message)
        {
            Logger.Log(LogPrefix + message);
        }



        internal static void SetupLogging(string Directory)
        {
            var logFilePath = Path.Combine(Directory, "log.txt");

            try
            {
                ShutdownLogging();
                AddLogFileForLogger(logFilePath);
            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        internal static void ShutdownLogging()
        {
            if (logAppender == null)
            {
                return;
            }

            try
            {
                HBS.Logging.Logger.ClearAppender(ModName);
                logAppender.Flush();
                logAppender.Close();
            }
            catch
            {
            }

            logAppender = null;
        }

        private static void AddLogFileForLogger(string logFilePath)
        {
            try
            {
                logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);
                HBS.Logging.Logger.AddAppender(ModName, logAppender);

            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        #endregion

    }
}
