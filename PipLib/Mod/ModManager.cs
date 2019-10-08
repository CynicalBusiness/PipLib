using PipLib.Building;
using PipLib.Elements;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PipLib.Mod
{
    public static class ModManager
    {
        internal static Logging.ILogger Logger = PipLib.Logger.Fork("ModManager");

        internal static List<Type> modTypes = new List<Type>();
        internal static List<IPipMod> mods = new List<IPipMod>();

        internal static Dictionary<IPipMod, Dictionary<PipMod.Step, List<MethodInfo>>> stepHandlers = new Dictionary<IPipMod, Dictionary<PipMod.Step, List<MethodInfo>>>();

        internal static void LoadTypes (Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPipMod).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    PipLib.Logger.Verbose("Found PipLib mod at: {0}, {1}", type.FullName, assembly.FullName);
                    modTypes.Add(type);

                    // TODO optimize these a bit?
                    ElementManager.CollectDefs(assembly.GetTypes());
                    BuildingManager.CollectBuildingInfo(assembly.GetTypes());
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
                    var mod = (IPipMod)ctor.Invoke(new object[]{ });
                    Logger.Verbose("Instanced: {0}", mod.Name);
                    mods.Add(mod);
                    CollectSteps(mod, mod.GetType().Assembly.GetTypes());
                }
            }
        }

        internal static void DoStep(PipMod.Step step)
        {
            foreach (var mod in mods)
            {
                DoStep(step, mod);
            }
        }


        internal static void DoStep(PipMod.Step step, IPipMod mod)
        {
            var methods = GetStepHandlersFor(mod, step);
            if (methods.Count > 0)
            {
                Logger.Verbose("{0}: {1}", step.ToString().ToUpper(), mod.Name);
                foreach (var method in methods)
                {
                    Logger.Debug("Invoking step handler in: {0}", method.DeclaringType.FullName);
                    var args = method.GetParameters();
                    if (args.Length > 0 && typeof(IPipMod).IsAssignableFrom(args[0].ParameterType))
                    {
                        method.Invoke(null, new object[]{ mod });
                    }
                    else
                    {
                        method.Invoke(null, new object[0]);
                    }
                }
            }
        }

        internal static void CollectSteps (IPipMod mod, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var stepAttrs = (PipMod.OnStep[])method.GetCustomAttributes(typeof(PipMod.OnStep), true);
                    if (stepAttrs.Length > 0)
                    {
                        foreach (var stepAttr in stepAttrs)
                        {
                            GetStepHandlersFor(mod, stepAttr.Step).Add(method);
                        }
                    }
                }
            }
        }

        private static List<MethodInfo> GetStepHandlersFor (IPipMod mod, PipMod.Step step)
        {
            var steps = GetStepHandlersFor(mod);
            if (!steps.TryGetValue(step, out var methods))
            {
                methods = new List<MethodInfo>();
                steps.Add(step, methods);
            }
            return methods;
        }

        private static Dictionary<PipMod.Step, List<MethodInfo>> GetStepHandlersFor (IPipMod mod)
        {
            if (!stepHandlers.TryGetValue(mod, out var handlers))
            {
                handlers = new Dictionary<PipMod.Step, List<MethodInfo>>();
                stepHandlers.Add(mod, handlers);
            }
            return handlers;
        }

        [PipMod.OnStep(PipMod.Step.Load)]
        private static void Load()
        {
            foreach (var mod in mods)
            {
                Logger.Info("Loading {0} v{1}", mod.Name, mod.Version);
                mod.Load();
            }
        }

        [PipMod.OnStep(PipMod.Step.PostLoad)]
        private static void PostLoad()
        {
            foreach (var mod in mods)
            {
                mod.PostLoad();
            }
        }

        [PipMod.OnStep(PipMod.Step.PreInitialize)]
        private static void PreInitialize()
        {
            foreach (var mod in mods)
            {
                mod.PreInitialize();
            }
        }

        [PipMod.OnStep(PipMod.Step.Initialize)]
        private static void Initialize()
        {
            foreach (var mod in mods)
            {
                mod.Initialize();
            }
        }

        [PipMod.OnStep(PipMod.Step.PostInitialize)]
        private static void PostInitialize()
        {
            foreach (var mod in mods)
            {
                mod.PostInitialize();
            }
        }
    }
}
