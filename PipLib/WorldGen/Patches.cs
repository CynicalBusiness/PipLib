using Harmony;
using ProcGen;
using System;

namespace PipLib.WorldGen
{
    public static class Patches
    {

        [HarmonyPatch(typeof(ComposableDictionary<string, ElementBandConfiguration>), "Merge")]
        private static class Patch_TerrainBiomeLookupTable_Merge
        {
            private static bool Prefix (ComposableDictionary<string, ElementBandConfiguration> __instance, ComposableDictionary<string, ElementBandConfiguration> other)
            {
                Traverse.Create(__instance).Method("VerifyConsolidated", new Type[0]).GetValue();

                foreach (var key in other.remove)
                {
                    __instance.add.Remove(key);
                }
                foreach (var keypair in other.add)
                {
                    if (__instance.add.TryGetValue(keypair.Key, out var exists))
                    {
                        exists.AddRange(keypair.Value);
                    }
                    else
                    {
                        __instance.Add(keypair.Key, keypair.Value);
                    }
                }

                return false;
            }
        }

    }
}
