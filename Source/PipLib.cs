using System.Collections.Generic;
using PipLib.Mod;

namespace PipLib
{
    public class PipLib
    {
        public static readonly string Version = typeof(PipLib).Assembly.GetName().Version.ToString();
        public static readonly string Product = typeof(PipLib).Assembly.GetName().Name;

        internal static readonly List<PipMod> mods = new List<PipMod>();
        internal static readonly ILogger logger = GlobalLogger.Get().Fork("PipLib");

        public static void Add(PipMod mod)
        {
            logger.Info("Added: ", mod.name);
        }

        internal static void Load ()
        {
            logger.Info("== Loading {0} v{1}", Product, Version);
            Patches.RegisterAll();
        }
    }
}
