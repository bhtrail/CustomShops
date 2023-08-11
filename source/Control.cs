using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BattleTech.UI;
using CustomShops.Patches;
using HBS.Logging;
using HBS.Util;
using CustomShops.Shops;

namespace CustomShops;

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

    internal static List<ISellShop> SaleShops = new List<ISellShop>();

    private static Dictionary<string, List<IShopDescriptor>> RefreshEvents;

    public static BuyBackShop BuyBack { get; private set; }
    public static void Init(string directory, string settingsJSON)
    {
        Logger = HBS.Logging.Logger.GetLogger(ModName, LogLevel.Debug);

        try
        {
            try
            {
                Settings = new CustomShopsSettings();
                JSONSerializationUtility.FromJSON(Settings, settingsJSON);
            }
            catch (Exception)
            {
                Settings = new CustomShopsSettings();
            }

            if (!Settings.AddLogPrefix)
            {
                LogPrefix = "";
            }

            HarmonyInit();

            Logger.Log("=========================================================");
            Logger.Log($"Loaded {ModName} v0.3.2 for bt 1.9");
            Logger.Log("=========================================================");
            Logger.LogDebug("done");
            Logger.LogDebug(JSONSerializationUtility.ToJSON(Settings));

            State = new GameState();
            RefreshEvents = new Dictionary<string, List<IShopDescriptor>>();


            RegisterRefreshEvent("Daily");
            RegisterRefreshEvent("SystemChange");
            RegisterRefreshEvent("MonthEnd");
            RegisterRefreshEvent("ContractComplete");
            RegisterRefreshEvent("OwnerChange");

            if (Settings.SystemShop)
            {
                RegisterShop(new SystemShop(), new List<string>() { "systemchange", "monthend" });
            }

            if (Settings.FactionShop)
            {
                RegisterShop(new FactionShop(), new List<string>() { "systemchange", "monthend" });
            }

            if (Settings.BlackMarketShop)
            {
                RegisterShop(new BlackMarketShop(), new List<string>() { "systemchange", "monthend" });
            }

            if (Settings.BuyBackShop)
            {
                BuyBack = new BuyBackShop();
                RegisterShop(BuyBack, new List<string>() { "systemchange" });
            }

        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    private static void HarmonyInit()
    {
        var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModName);

        var appdomain = AppDomain.CurrentDomain;
        var assmbls = appdomain.GetAssemblies();
        var irt = assmbls.FirstOrDefault(i => i.GetName().Name == "IRTweaks");
        if (irt == null)
        {
            Log("No IRTweaks found, usual shop button");
            var method_base = AccessTools.Method(typeof(SGNavigationButton), "ManageShopFlyout");
            var method_new = new HarmonyMethod(typeof(SGNavigationButton_ManageShopFlyout), "ShowButton");
            harmony.Patch(method_base, method_new);
        }
        else
        {
            Log("IRTweaks found, patching");
            var refresh_patch =
                irt.GetTypes().FirstOrDefault(i => i.Name == "SGNavigationList_RefreshButtonStates");
            if (refresh_patch == null)
            {
                LogError("Cannot find IRTweak class to patch");
                return;
            }

            var method_base = AccessTools.Method(refresh_patch, "IsShopActive");
            var method_new = new HarmonyMethod(typeof(IsShopActive), "ShowButton");
            harmony.Patch(method_base, method_new);
            
        }
        Log("Patched");
    }

    public static string GetMDefFromCDef(string id)
    {
        return id.Replace("chassis", "mech");
    }

    private static bool register_shop(IShopDescriptor shop)
    {
        if (shop == null)
        {
            return false;
        }

        Log($"Shop [{shop.Name}] registred");
        Shops.Add(shop);

        if (shop is ISellShop ss)
        {
            SaleShops.Add(ss);
            SaleShops.Sort((i1, i2) => i1.SellPriority.CompareTo(i2.SellPriority));
        }

        return true;

    }

    [Obsolete]
    public static void RegisterShop(IShopDescriptor shop)
    {
        if (register_shop(shop))
        {
            if (shop.RefreshOnMonthChange)
            {
                AddToEvent("MonthEnd", shop);
            }

            if (shop.RefreshOnOwnerChange)
            {
                AddToEvent("OwnerChange", shop);
            }

            if (shop.RefreshOnSystemChange)
            {
                AddToEvent("SystemChange", shop);
            }
        }

    }

    public static void RegisterShop(IShopDescriptor shop, IEnumerable<string> refresh_events)
    {
        if (register_shop(shop) && refresh_events != null)
        {
            foreach (var name in refresh_events)
            {
                AddToEvent(name, shop);
            }
        }
    }

    #region LOGGING
    [Conditional("CCDEBUG")]
    public static void LogDebug(DInfo type, string message)
    {
        if (Settings.DebugType.HasFlag(type))
        {
            Logger.LogDebug(LogPrefix + message);
        }
    }
    [Conditional("CCDEBUG")]
    public static void LogDebug(DInfo type, string message, Exception e)
    {
        if (Settings.DebugType.HasFlag(type))
        {
            Logger.LogDebug(LogPrefix + message, e);
        }
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

    public static void RegisterRefreshEvent(string Event)
    {
        var n = Event.ToLower();
        if (RefreshEvents.ContainsKey(n))
        {
            LogError($"Refresh event {Event} already registred");
            return;
        }
        RefreshEvents[n] = new List<IShopDescriptor>();

    }

    internal static void AddToEvent(string name, IShopDescriptor shop)
    {
        var n = name.ToLower();
        if (!RefreshEvents.ContainsKey(n))
        {
            LogError($"Unknown Refresh event {name}, please check if registred");
            RegisterRefreshEvent(name);
        }

        RefreshEvents[n].Add(shop);
    }

    public static void RefreshShops(string Event)
    {


        if (RefreshEvents.TryGetValue(Event.ToLower(), out var shops))
        {
            foreach (var shopDescriptor in shops)
            {
                shopDescriptor.RefreshShop();
            }
        }
        else
        {
            LogError($"Unknown Refresh event {Event} requested, skipped");
        }
    }
}
