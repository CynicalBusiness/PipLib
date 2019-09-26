using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;
using PipLib.Mod;

namespace PipLib.Patches
{
    public static class Elements
    {

        [HarmonyPatch(typeof(Enum))]
        [HarmonyPatch(nameof(Enum.ToString))]
        [HarmonyPatch(new Type[] { })]
        internal static class SimHashes_ToString_Patch
        {
            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes))
                {
                    return true;
                }

                return !PipLib.simHashTable.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(Enum))]
        [HarmonyPatch(nameof(Enum.Parse))]
        [HarmonyPatch(new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        internal static class SimHashes_Parse_Patch
        {
            private static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (!enumType.Equals(typeof(SimHashes)))
                {
                    return true;
                }

                return !PipLib.simHashReverseTable.TryGetValue(value, out __result);
            }
        }

        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.CollectElementsFromYAML))]
        internal static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                PipLib.Logger.Info("Injecting elements...");
                foreach (var mod in PipLib.Mods)
                {
                    var pooledList = ListPool<FileHandle, ElementLoader>.Allocate();
                    FileSystem.GetFiles(FileSystem.Normalize(AssetLoader.GetAssemblyDirectory(mod) + "/elements"), "*.yml", pooledList);
                    foreach (var file in pooledList)
                    {
                        Debug.Log(file.full_path);
                        var elementCollection = YamlIO.Parse<ElementLoader.ElementEntryCollection>(File.ReadAllText(file.full_path), Path.GetFileName(file.full_path));
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


        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                PipLib.Logger.Info("Registering substances...");
                foreach (var mod in PipLib.Mods)
                {
                    foreach (var element in mod.GetElements())
                    {
                        element.RegisterSubstances(substanceList, substanceTable);
                    }
                }
            }
        }
    }
}
