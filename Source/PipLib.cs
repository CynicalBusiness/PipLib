using System.Collections.Generic;
using PipLib.Mod;

namespace PipLib
{
    public static class PipLib
    {
        public static readonly string Version = typeof(PipLib).Assembly.GetName().Version.ToString();
        public static readonly string Product = typeof(PipLib).Assembly.GetName().Name;

        internal static readonly List<PipMod> mods = new List<PipMod>();
        internal static readonly ILogger logger = GlobalLogger.Get().Fork("PipLib");

        private static bool loaded = false;

        public static void Add(PipMod mod)
        {
            logger.Info("Added: {0}", mod.name);
            mods.Add(mod);
        }

        public static ElementFactory CreateElement(PipMod mod, string name)
        {
            return new ElementFactory(new PrefixedId(mod, name));
        }

        public static BuildingFactory CreateBuilding (PipMod mod, string name)
        {
            return new BuildingFactory(new PrefixedId(mod, name));
        }

        internal static void Load()
        {
            if (!loaded)
            {
                GlobalLogger.Get().Info("== Loading {0} v{1}", Product, Version);
                foreach (var mod in mods)
                {
                    mod.logger.Info("= Loading {0}", mod.name);
                    mod.Load();
                }
                Patches.RegisterAll();
                loaded = true;
            }
        }
    }
}
