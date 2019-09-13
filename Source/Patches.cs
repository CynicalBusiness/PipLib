using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;
using PipLib.Mod;

namespace PipLib
{
    public class Patches
    {

        public static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        public static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        public static void RegisterAll()
        {
            foreach (var mod in PipLib.mods)
            {
                mod.RegisterSimHashes(simHashTable, simHashReverseTable);
                mod.RegisterStrings();
            }
        }

        [HarmonyPatch(typeof(GlobalResources))]
        [HarmonyPatch(nameof(GlobalResources.Instance))]
        private static class Patch_GlobalResources_Instance
        {
            private static void Postfix()
            {
                PipLib.Load();
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        private static class Patch_Db_Initialize
        {

            private static void Postfix()
            {
                GlobalLogger.Get().Info("Applying substance attributes...");
                foreach (var mod in PipLib.mods)
                {
                    mod.RegisterAttributes();
                }
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
                PipLib.logger.Info("Injecting elements...");
                foreach (var mod in PipLib.mods)
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
                PipLib.logger.Info("Done injecting.");
            }
        }


        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        private static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                GlobalLogger.Get().Info("Registering substances...");
                foreach (var mod in PipLib.mods)
                {
                    mod.RegisterSubstances(substanceList, substanceTable);
                }
            }
        }
    }

    public class UnknownPipObjectException : Exception
    {
        public UnknownPipObjectException(PipObject obj) : base($"Tried adding unknown PipLib object: {obj.GetType().FullName}") { }
    }

}
