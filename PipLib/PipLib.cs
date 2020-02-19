using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using PipLib.Mod;
using PipLib.Options;
using Newtonsoft.Json;
using static PipLib.PLUtil;

// TODO do something like PLib to manage multiple versions

namespace PipLib
{
    public static class PipLib
    {
        public static readonly string Version = GetCurrentVersion();
        public static readonly string Name = GetCurrentName();

        internal static readonly Logging.ILogger Logger = Logging.GlobalLogger.Get().Fork("PipLib");

        internal static LibOptions.Provider OptionsProvider { get; private set; }

        public static LibOptions Options => OptionsProvider.Options;

        static PipLib()
        {
            // there are cases, such as the logs that output during options reading, that things want to access this
            // before it has loaded
            // so, we're going to initialize it to the defaults first, then overwrite it
            OptionsProvider = new LibOptions.Provider();
        }

        public static void PrePatch(HarmonyInstance _)
        {
            // this is weird and should not be done in normal mods
            OptionsManager.MergeOptions(OptionsProvider);
            OptionsManager.SaveOptions(OptionsProvider);
            if (Options.EnableDeveloperConsole)
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

        public static IEnumerable<IPipMod> Mods => ModManager.Mods;
    }
}
