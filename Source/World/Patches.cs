using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;
using PipLib.Asset;
using PipLib.Mod;

namespace PipLib.World
{
    public class WorldPatches
    {

        public static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        public static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        public static void RegisterAll(PipMod mod)
        {
            foreach (var e in mod.elements)
            {
                e.RegisterSimHashes(simHashTable, simHashReverseTable);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        internal static class Patch_Db_Initialize
        {
            private static void Postfix()
            {
                Debug.Log("Applying substance attributes...");
                foreach (var mod in PipLib.mods)
                {
                    foreach (var e in mod.elements)
                    {
                        Debug.Log($"* {e.id}");
                        e.RegisterAttributes();
                    }
                }

                Debug.Log("Done applying attributes");
            }
        }

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

                return !simHashTable.TryGetValue((SimHashes)__instance, out __result);
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

                return !simHashReverseTable.TryGetValue(value, out __result);
            }
        }

        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.CollectElementsFromYAML))]
        internal static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Debug.Log("Injecting elements...");
                foreach (var mod in PipLib.mods)
                {
                    var pooledList = ListPool<FileHandle, ElementLoader>.Allocate();
                    FileSystem.GetFiles(FileSystem.Normalize(AssetLoader.GetAssemblyDirectory(mod) + "/element"), "*.yaml", pooledList);
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
                    /* var loaded = YamlIO.Parse<ElementLoader.ElementEntryCollection>(mod.GetAsset<TextAsset>(AssetLoader.ELEMENTS).text, null);
                    foreach (var e in loaded.elements)
                    {
                        Debug.Log($"* {e.elementId}");
                        __result.Add(e);
                    } */
                }
                Debug.Log("Done injecting.");
            }
        }


        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                Debug.Log("Registering substances...");
                int numElements = 0, numSubstances = 0;
                foreach (var mod in PipLib.mods)
                {
                    Debug.Log($"* {mod.name}");
                    foreach (var e in mod.elements)
                    {
                        var states = e.States;
                        if (states.Count > 0)
                        {
                            Debug.Log($"** {e}");
                            e.RegisterSubstances(substanceList, substanceTable);
                            numElements++;
                            numSubstances += states.Count;
                        }
                        else
                        {
                            Debug.LogWarning($"{e.id} has no states");
                        }
                    }
                }

                Debug.Log($"Done registering {numSubstances} substances for {numElements} elements");
            }
        }
    }

    public class UnknownPipObjectException : Exception
    {
        public UnknownPipObjectException(PipObject obj) : base($"Tried adding unknown PipLib object: {obj.GetType().FullName}") { }
    }

}
