using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;

namespace PipLib.Elements
{
    public static class Patches
    {

        [HarmonyPatch(typeof(Enum), "ToString", new Type[]{ })]
        internal static class Patch_Enum_ToString
        {
            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (__instance is SimHashes)
                {
                    return !ElementManager.simHashTable.TryGetValue((SimHashes)__instance, out __result);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        internal static class Patch_Enum_Parse
        {
            private static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (enumType.Equals(typeof(SimHashes)))
                {
                    return !ElementManager.simHashReverseTable.TryGetValue(value, out __result);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Enum), "GetValues", new Type[]{ typeof(Type) })]
        private static class Patch_Enum_GetValues
        {
            private static void Postfix(Type enumType, ref Array __result)
            {
                if (enumType.Equals(typeof(SimHashes)))
                {
                    var res = new List<SimHashes>();
                    res.AddRange((SimHashes[]) __result);
                    res.AddRange(ElementManager.simHashTable.Keys);
                    __result = res.ToArray();
                }
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        internal static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                PipLib.Logger.Info("Loading elements...");
                foreach (var mod in PipLib.Mods)
                {
                    ElementManager.CollectElements(mod, ref __result);
                }
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "Load")]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                ElementManager.Logger.Info("Registering substances...");
                ElementManager.RegisterSubstances(substanceList, substanceTable);
            }
        }
    }
}
