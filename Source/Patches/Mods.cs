using System;
using System.Reflection;
using Harmony;

namespace PipLib.Patches
{
    public static class Mods
    {

        [HarmonyPatch(typeof(HarmonyInstance))]
        [HarmonyPatch("PatchAll")]
        [HarmonyPatch(new Type[] { typeof(Assembly) })]
        private static class Patch_HarmonyInstance_PatchAll
        {
            // Used to get a copy of all mod assemblies that load after ours, in order
            private static void Postfix(Assembly assembly)
            {
                PipLib.Logger.Verbose("Queueing assembly: {0}", assembly.GetName());
                PipLib.modAssemblies.Add(assembly);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        private static class Patch_Db_Initialize
        {

            private static void Prefix()
            {
                PipLib.PreInitialize();
            }

            private static void Postfix()
            {
                PipLib.Initialize();
                PipLib.PostInitialize();
            }
        }

        [HarmonyPatch(typeof(GlobalResources))]
        [HarmonyPatch(nameof(GlobalResources.Instance))]
        private static class Patch_GlobalResources_Instance
        {
            // GlobalResources.Instance happens at the end of Global.Awake
            // Since we can't patch what's currently in the call stack, this will have to do

            private static void Prefix()
            {
                PipLib.LoadMods();
                PipLib.Load();
            }

            private static void Postfix()
            {
                PipLib.PostLoad();
            }
        }
    }
}
