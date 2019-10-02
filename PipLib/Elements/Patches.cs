using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;
using PipLib.Mod;

namespace PipLib.Elements
{
    public static class Patches
    {

        [HarmonyPatch(typeof(Enum), "ToString", new Type[]{ })]
        internal static class Patch_SimHashes_ToString
        {
            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes))
                {
                    return true;
                }

                return !ElementLoader.simHashTable.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        internal static class Patch_SimHashes_Parse
        {
            private static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (!enumType.Equals(typeof(SimHashes)))
                {
                    return true;
                }

                return !ElementLoader.simHashReverseTable.TryGetValue(value, out __result);
            }
        }

        [HarmonyPatch(typeof(global::ElementLoader), "CollectElementsFromYAML")]
        internal static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<global::ElementLoader.ElementEntry> __result)
            {
                PipLib.Logger.Info("Injecting elements...");
                foreach (var mod in PipLib.Mods)
                {
                    var pooledList = ListPool<FileHandle, global::ElementLoader>.Allocate();
                    FileSystem.GetFiles(Path.Combine(PLUtil.GetAssemblyDir(mod.GetType()), PLUtil.DIR_ELEMENTS), "*.yml", pooledList);
                    foreach (var file in pooledList)
                    {
                        PipLib.Logger.Debug("loading elements from: {0}", file.full_path);
                        var elementCollection = YamlIO.Parse<global::ElementLoader.ElementEntryCollection>(File.ReadAllText(file.full_path), Path.GetFileName(file.full_path));
                        if (elementCollection != null)
                        {
                            __result.AddRange(elementCollection.elements);
                        }
                    }
                    pooledList.Recycle();
                }
                PipLib.Logger.Info("Done injecting.");
            }
        }


        [HarmonyPatch(typeof(global::ElementLoader), "Load")]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                PipLib.Logger.Info("Registering substances...");
                ElementLoader.RegisterSubstances(substanceList, substanceTable);
            }
        }
    }
}
