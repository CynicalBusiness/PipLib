using System;
using System.Diagnostics;
using System.Reflection;
using Harmony;

namespace PipLib.Mod
{
    public static class Mods
    {

        [HarmonyPatch(typeof(HarmonyInstance))]
        [HarmonyPatch("PatchAll")]
        [HarmonyPatch(new Type[] { typeof(Assembly) })]
        private static class Patch_HarmonyInstance_PatchAll
        {
            private static void Postfix(HarmonyInstance __instance, Assembly assembly)
            {
                // this is kinda a dangerous thing to patch, so we only want to patch specifically what we're looking for
                // to do that, we move up two stack frames and only actually _do_ anything if we're in DLLLoader.LoadDlls

                var frame = new StackFrame(2); // once for the harmony patch wrapper and once for the method itself
                PipLib.Logger.Debug("Trying to patch from HarmonyInstance with ID: {0} (frame +2: {1}.{2})", __instance.Id, frame.GetMethod().ReflectedType.Name, frame.GetMethod().Name);
                if (__instance.Id == "OxygenNotIncluded_v0.1" && frame.GetMethod().Name == "LoadDLLs")
                {
                    PipLib.Logger.Verbose("Scanning assembly: {0}", assembly.GetName().Name);
                    ModManager.LoadTypes(assembly);
                }
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        private static class Patch_Db_Initialize
        {

            private static void Prefix()
            {
                ModManager.DoStep(PipMod.Step.PreInitialize);
            }

            private static void Postfix()
            {
                ModManager.DoStep(PipMod.Step.Initialize);
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        private static class PatchLast_Db_Initialize
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix()
            {
                ModManager.DoStep(PipMod.Step.PostInitialize);
            }
        }

        [HarmonyPatch(typeof(Global))]
        [HarmonyPatch("RestoreLegacyMetricsSetting")]
        private static class Patch_Global_RestoreLegacyMetricsSetting
        {
            // This happens at the end of Global.Awake
            // Since we can't patch what's currently in the call stack, this will have to do

            private static void Postfix()
            {
                ModManager.InstanciateAll();
                ModManager.DoStep(PipMod.Step.Load);
                ModManager.DoStep(PipMod.Step.PostLoad);
            }
        }

        [HarmonyPatch(typeof(ModUtil), "AddKAnimMod")]
        private static class Patch_ModUtil_AddKAnimMod
        {
            // all this does is output the name of the loaded animations for debugging
            private static void Postfix(KAnimFile __result)
            {
                ModManager.Logger.Verbose("Loaded anim '{0}'", __result.name);
            }
        }
    }
}
