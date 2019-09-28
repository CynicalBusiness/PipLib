using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using PeterHan.PLib.Options;
using PipLib.Mod;
using PeterHan.PLib;

namespace PipLib
{
    public static class PipLib
    {
        public static readonly string Version = typeof(PipLib).Assembly.GetName().Version.ToString();
        public static readonly string Product = typeof(PipLib).Assembly.GetName().Name;

        internal static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        internal static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        internal static readonly ILogger Logger = GlobalLogger.Get().Fork("PipLib");
        internal static Dictionary<string, IPipMod> mods;

        internal static List<Type> modTypes = new List<Type>();
        internal static List<Assembly> modAssemblies = new List<Assembly>();

        public static PipLibOptions Options { get; private set; }

        public static void PrePatch(HarmonyInstance _)
        {
            PUtil.LogModInit();
            POptions.RegisterOptions(typeof(PipLibOptions));
            Options = POptions.ReadSettings<PipLibOptions>() ?? new PipLibOptions();
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
            modAssemblies.Insert(0, Assembly.GetExecutingAssembly());
        }

        public static IEnumerable<IPipMod> Mods => mods.Values;

        private static List<Type> GetAllModImplementations()
        {
            var impl = new List<Type>();
            foreach (var assembly in modAssemblies)
            {
                bool found = false;
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPipMod).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        impl.Add(type);
                        found = true;
                    }
                }
                if (!found)
                {
                    Logger.Verbose("Assembly \"{0}\" does not contain PipLib Mod, not handling it...", assembly.GetName().FullName);
                }
            }
            return impl;
        }

        private static List<IPipMod> GetAllMods()
        {
            return GetAllModImplementations().ConvertAll(t => (IPipMod)Activator.CreateInstance(t));
        }

        private static Dictionary<string, IPipMod> GetMods()
        {
            var dict = new Dictionary<string, IPipMod>();
            foreach (var mod in GetAllMods())
            {
                dict.Add(mod.Name, mod);
            }
            return dict;
        }

        internal static void LoadMods()
        {
            mods = GetMods();
        }

        internal static void Load()
        {
            Logger.Info("Starting mod Load");
            foreach (var mod in Mods)
            {
                Logger.Info("Loading {0} v{1}", mod.Name, mod.Version);
                mod.Load();
            }
        }

        internal static void PostLoad()
        {
            Logger.Info("Starting mod PostLoad");
            foreach (var mod in Mods)
            {
                mod.PostLoad();
            }
        }

        internal static void PreInitialize()
        {
            Logger.Info("Starting mod PreInitialize");
            foreach (var mod in Mods)
            {
                mod.PreInitialize();
            }
        }

        internal static void Initialize()
        {
            Logger.Info("Starting mod Initialize");
            foreach (var mod in Mods)
            {
                mod.Initialize();
            }
        }

        internal static void PostInitialize()
        {
            Logger.Info("Starting mod PostInitialize");
            foreach (var mod in Mods)
            {
                mod.PostInitialize();
            }
        }
    }
}
