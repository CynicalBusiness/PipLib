using Harmony;

namespace PipLib.Tech
{
    public static class TechPatches
    {

        [HarmonyPatch(typeof(Db), "Initialize")]
        private static class PatchFirst_Db_Initialize
        {
            [HarmonyPriority(Priority.First)]
            private static void Postfix (Db __instance)
            {
                TechTree.Create(__instance.Techs);
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        private static class PatchLate_Db_Initialize
        {
            [HarmonyPriority(Priority.VeryLow)]
            private static void Postfix (Db __instance)
            {
                TechTree.Instance.RebuildArragement();
            }
        }

    }
}
