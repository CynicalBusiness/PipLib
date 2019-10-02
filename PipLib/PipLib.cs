using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using PipLib.Mod;
using static PipLib.PLUtil;

// TODO do something like PLib to manage multiple versions

namespace PipLib
{
    public static class PipLib
    {

        public static readonly string Version = GetCurrentVersion();
        public static readonly string Name = GetCurrentName();

        internal static readonly Logging.ILogger Logger = Logging.GlobalLogger.Get().Fork("PipLib");

        public static PLOptions Options { get; private set; }

        public static void PrePatch(HarmonyInstance _)
        {
            PUtil.LogModInit();
            POptions.RegisterOptions(typeof(PLOptions));
            Options = POptions.ReadSettings<PLOptions>() ?? new PLOptions();
            if (Options.enableDeveloperConsole)
            {
                UnityEngine.Debug.LogError("Invoking error to show the developer console");
            }
            Logger.Info("Using options: {0}", Options.ToString());

            Logger.Info("Hooked into harmony, watching for assemblies to be patched in...");
        }

        public static void PostPatch(HarmonyInstance _)
        {
            // add our assembly in first, since the patch will miss it
            ModManager.LoadTypes(Assembly.GetExecutingAssembly());
        }

        public static void OnLoad ()
        {
            LocString.CreateLocStringKeys(typeof(global::PipLib.STRINGS.ELEMENTS));
            LocString.CreateLocStringKeys(typeof(global::PipLib.STRINGS.RESEARCH));
        }

        public static IEnumerable<IPipMod> Mods => ModManager.mods;
    }
}
