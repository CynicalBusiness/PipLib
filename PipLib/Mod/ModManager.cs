using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PipLib.Mod
{
    public static class ModManager
    {
        internal static Logging.ILogger Logger = PipLib.Logger.Fork("ModManager");

        internal static Dictionary<Assembly, Tuple<IPipMod, Type>> mods = new Dictionary<Assembly, Tuple<IPipMod, Type>>();

        internal static Dictionary<Assembly, Dictionary<PipMod.Step, List<MethodInfo>>> stepHandlers = new Dictionary<Assembly, Dictionary<PipMod.Step, List<MethodInfo>>>();

        internal static Dictionary<PipMod.TypeCollector, MethodInfo> collectors = new Dictionary<PipMod.TypeCollector, MethodInfo>();

        internal static List<Tuple<bool, Assembly>> assemblies = new List<Tuple<bool, Assembly>>();

        public static IEnumerable<IPipMod> Mods
        {
            get
            {
                return
                    from mod in mods.Values
                    where mod.first != null
                    select mod.first;
            }
        }

        public static IEnumerable<Type> ModTypes
        {
            get
            {
                return
                    from mod in mods.Values
                    select mod.second;
            }
        }

        public static IPipMod GetModForAssembly (Assembly assembly)
        {
            return mods.TryGetValue(assembly, out var mod) ? mod.first : null;
        }

        public static bool IsPipModAssembly (Assembly assembly)
        {
            return GetModForAssembly(assembly) != null;
        }

        internal static void LoadTypes (Assembly assembly)
        {
            bool isPipModAssembly = false;
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (PipMod.TypeCollector.IsImpl(typeof(IPipMod), type))
                {
                    PipLib.Logger.Verbose("Found PipLib mod at: {0}, {1}", type.FullName, assembly.FullName);
                    mods.Add(assembly, new Tuple<IPipMod, Type>(null, type));
                    isPipModAssembly = true;
                }

                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var p = method.GetParameters();
                    if (p.Length != 1 || !p[0].ParameterType.Equals(typeof(Type)))
                    {
                        continue;
                    }

                    var collectorAttrs = (PipMod.TypeCollector[])method.GetCustomAttributes(typeof(PipMod.TypeCollector), true);
                    if (collectorAttrs.Length > 0)
                    {
                        foreach (var c in collectorAttrs)
                        {
                            Logger.Verbose("Registed type collector {0} for type: {1}", method.DeclaringType.FullName, c.Predicate.FullName);
                            collectors.Add(c, method);
                        }
                    }
                }
            }

            assemblies.Add(new Tuple<bool, Assembly>(isPipModAssembly, assembly));
            CollectTypes(types, isPipModAssembly);
            CollectSteps(assembly);
        }

        internal static void CollectTypes (Type[] types, bool isPipModAssembly)
        {
            foreach (var (attr, method) in collectors)
            {
                int total = 0, found = 0;
                foreach (var type in types)
                {
                    total++;
                    if (PipMod.TypeCollector.IsImpl(attr.Predicate, type))
                    {
                        if (isPipModAssembly || attr.CaptureNonPipTypes)
                        {
                            found++;
                            Logger.Debug("Invoked type collector {0} on: {1}", method.DeclaringType.FullName, type.FullName);
                            method.Invoke(null, new object[1]{ type });
                        }
                        else
                        {
                            Logger.Debug("Type collector {0} found `{1}`, but was skipped by filtering", method.DeclaringType.FullName, type.FullName);
                        }
                    }
                }

                Logger.Debug("Collector searched {0} type(s) (should be {1}), {2} found matching predicate", total, types.Length, found);
            }
        }

        internal static void InstanciateAll ()
        {
            foreach (var (assembly, (modInst, modType)) in mods)
            {
                if (modInst != null) continue;

                var ctor = modType.GetConstructor(new Type[]{ });
                if (ctor == null)
                {
                    Logger.Warning("Could not instanciate '{0}' because no constructor with empty arguments exists", modType.FullName);
                }
                else
                {
                    var mod = (IPipMod)ctor.Invoke(new object[]{ });
                    Logger.Verbose("Instanced: {0}", mod.Name);
                    mods[assembly].first = mod;
                    DoStep(PipMod.Step.PostInstanciate, mod);
                }
            }
        }

        internal static KMod.Mod GetKMod (IPipMod mod)
        {
            return Global.Instance.modManager.mods.Find(m => m.label.id == System.IO.Path.GetFileName(PLUtil.GetAssemblyDir(mod.GetType())));
        }

        internal static void ModHadError (IPipMod mod, PipMod.Step step)
        {
            /* var kmod = GetKMod(mod);
            if (kmod.label.distribution_platform != KMod.Label.DistributionPlatform.Dev && kmod.label.distribution_platform != KMod.Label.DistributionPlatform.Local)
            {
                kmod.status = KMod.Mod.Status.ReinstallPending;
            }
            Logger.Debug(kmod.status.ToString());
            KMod.Manager.Dialog(
                title: STRINGS.UI.FRONTEND.MOD_DIALOGS.STEP_FAILURE.TITLE,
                text: string.Format(STRINGS.UI.FRONTEND.MOD_DIALOGS.STEP_FAILURE.MESSAGE, step.ToString().ToUpper(), mod.Name),
                confirm_text: global::STRINGS.UI.FRONTEND.MOD_DIALOGS.RESTART.OK,
                on_confirm: App.instance.Restart,
                cancel_text: global::STRINGS.UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL,
                on_cancel: PLUtil.NOOP,
                activateBlackBackground: true
            ); */
        }

        internal static void DoStep(PipMod.Step step)
        {
            foreach (var (isPipMod, assembly) in assemblies)
            {
                DoStep(step, assembly);
            }
        }

        internal static void DoStep(PipMod.Step step, IPipMod mod)
        {
            DoStep(step, mod.GetType().Assembly);
        }

        internal static void DoStep(PipMod.Step step, Assembly assembly)
        {
            var methods = GetStepHandlersFor(assembly, step);
            if (methods.Count > 0)
            {
                Logger.Verbose("{0}: {1}", step.ToString().ToUpper(), assembly.FullName);
                foreach (var method in methods)
                {
                    try
                    {
                        Logger.Debug("Invoking step handler in: {0}", method.DeclaringType.FullName);
                        var args = method.GetParameters();
                        if (args.Length == 1 && typeof(IPipMod).IsAssignableFrom(args[0].ParameterType))
                        {
                            method.Invoke(null, new object[]{ GetModForAssembly(assembly) });
                        }
                        else if (args.Length == 1 && typeof(Assembly).IsAssignableFrom(args[0].ParameterType))
                        {
                            method.Invoke(null, new object[]{ assembly });
                        }
                        else
                        {
                            method.Invoke(null, new object[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed running {0} for '{1}' at {2}.{3}:", step.ToString().ToUpper(), assembly.FullName, method.DeclaringType.FullName, method.Name);
                        Logger.Log(ex);
                        // ModHadError(mod, step);
                    }
                }
            }
        }

        internal static void CollectSteps (Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var stepAttrs = (PipMod.OnStep[])method.GetCustomAttributes(typeof(PipMod.OnStep), true);
                    if (stepAttrs.Length > 0)
                    {
                        foreach (var stepAttr in stepAttrs)
                        {
                            GetStepHandlersFor(assembly, stepAttr.Step).Add(method);
                        }
                    }
                }
            }
        }

        private static List<MethodInfo> GetStepHandlersFor (IPipMod mod, PipMod.Step step)
        {
            return GetStepHandlersFor(mod.GetType().Assembly, step);
        }

        private static Dictionary<PipMod.Step, List<MethodInfo>> GetStepHandlersFor (IPipMod mod)
        {
            return GetStepHandlersFor(mod.GetType().Assembly);
        }

        private static List<MethodInfo> GetStepHandlersFor (Assembly assembly, PipMod.Step step)
        {
            var handlersForAssembly = GetStepHandlersFor(assembly);
            if (!handlersForAssembly.TryGetValue(step, out var methods))
            {
                methods = new List<MethodInfo>();
                handlersForAssembly.Add(step, methods);
            }
            return methods;
        }

        private static Dictionary<PipMod.Step, List<MethodInfo>> GetStepHandlersFor (Assembly assembly)
        {
            if (!stepHandlers.TryGetValue(assembly, out var dict))
            {
                dict = new Dictionary<PipMod.Step, List<MethodInfo>>();
                stepHandlers.Add(assembly, dict);
            }
            return dict;
        }

        [PipMod.OnStep(PipMod.Step.Load)]
        private static void Load()
        {
            foreach (var (assembly, (mod, modType)) in mods)
            {
                Logger.Info("Loading {0} v{1} ({2})", mod.Name, mod.Version, assembly.FullName);
                mod.Load();
            }
        }

        [PipMod.OnStep(PipMod.Step.PostLoad)]
        private static void PostLoad()
        {
            foreach (var mod in Mods)
            {
                mod.PostLoad();
            }
        }

        [PipMod.OnStep(PipMod.Step.PreInitialize)]
        private static void PreInitialize()
        {
            foreach (var mod in Mods)
            {
                mod.PreInitialize();
            }
        }

        [PipMod.OnStep(PipMod.Step.Initialize)]
        private static void Initialize()
        {
            foreach (var mod in Mods)
            {
                mod.Initialize();
            }
        }

        [PipMod.OnStep(PipMod.Step.PostInitialize)]
        private static void PostInitialize()
        {
            foreach (var mod in Mods)
            {
                mod.PostInitialize();
            }
        }
    }
}
