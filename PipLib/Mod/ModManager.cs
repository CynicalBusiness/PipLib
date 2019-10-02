using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PipLib.Mod
{
    internal static class ModManager
    {
        internal static Logging.ILogger Logger = PipLib.Logger.Fork("ModManager");

        internal static List<Type> modTypes = new List<Type>();
        internal static List<IPipMod> mods = new List<IPipMod>();

        internal static void LoadTypes (Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPipMod).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    PipLib.Logger.Verbose("Found PipLib mod at: {0}, {1}", type.FullName, assembly.FullName);
                    modTypes.Add(type);

                    Elements.ElementLoader.GetDefs(assembly.GetTypes());
                }
            }
        }

        internal static void InstanciateAll ()
        {
            foreach (var type in modTypes)
            {
                var ctor = type.GetConstructor(new Type[]{ });
                if (ctor == null)
                {
                    Logger.Warning("Could not instanciate '{0}' because no constructor with empty arguments exists", type.FullName);
                }
                else
                {
                    Logger.Verbose("Instanced: {0}", type.FullName);
                    mods.Add((IPipMod)ctor.Invoke(new object[]{ }));
                }
            }
        }

        internal static void Load()
        {
            Logger.Info("Starting mod Load");
            foreach (var mod in mods)
            {
                Logger.Info("Loading {0} v{1}", mod.Name, mod.Version);
                if (mod.OptionsType != null)
                {
                    POptions.RegisterOptions(mod.OptionsType);
                }
                mod.Load();
            }
        }

        internal static void PostLoad()
        {
            Logger.Info("Starting mod PostLoad");
            foreach (var mod in mods)
            {
                mod.PostLoad();
            }
        }

        internal static void PreInitialize()
        {
            Logger.Info("Starting mod PreInitialize");
            foreach (var mod in mods)
            {
                mod.PreInitialize();
            }
        }

        internal static void Initialize()
        {
            Logger.Info("Starting mod Initialize");
            foreach (var mod in mods)
            {
                mod.Initialize();
            }
        }

        internal static void PostInitialize()
        {
            Logger.Info("Starting mod PostInitialize");
            foreach (var mod in mods)
            {
                mod.PostInitialize();
            }
        }
    }
}
