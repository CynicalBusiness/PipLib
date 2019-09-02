using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using Klei;
using PipLib.Asset;
using PipLib.Mod;
using UnityEngine;

namespace PipLib.World
{
    public class WorldPatches
    {

        private static readonly List<PipElement> elements = new List<PipElement>();

        public static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        public static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        public static void Add<T>(T obj) where T : PipObject
        {
            if (typeof(PipElement).IsAssignableFrom(typeof(T)))
            {
                elements.Add(obj as PipElement);
            }
            else
            {
                throw new UnknownPipObjectException(obj);
            }
        }

        public static void RegisterAll()
        {
            foreach (var e in elements)
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
                foreach (var m in elements)
                {
                    Debug.Log($"* {m.id}");
                    m.RegisterAttributes();
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

        // TODO move this elselwere?
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.CollectElementsFromYAML))]
        internal static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Debug.Log("Injecting elements...");
                foreach (var mod in PipLib.mods)
                {
                    var loaded = YamlIO.Parse<ElementLoader.ElementEntryCollection>(mod.GetAsset<TextAsset>(AssetLoader.ELEMENTS).text, null);
                    foreach (var e in loaded.elements)
                    {
                        Debug.Log($"* {e.elementId}");
                        __result.Add(e);
                    }
                }
                Debug.Log("Done injecting.");
            }
        }


        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        internal static class Patch_ElementLoader_Load
        {

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                Debug.Log("Updating KAnims...");
                // annoyingly, all of the included KAnim methods, namely ModUtil.AddKAnim, are late to the party
                // so we have to update the anim table again
                var animTable = Traverse.Create(typeof(Assets)).Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>();
                animTable.Clear();
                Assets.Anims.AddRange(Assets.ModLoadedKAnims);
                foreach (var anim in Assets.Anims)
                {
                    if (anim != null)
                    {
                        if (!animTable.ContainsKey(anim.name))
                        {
                            animTable.Add(anim.name, anim);
                        }
                        else
                        {
                            Debug.LogWarning("Tried to add already added anim: " + anim.name);
                        }
                    }
                }

                Debug.Log("Registering substances...");
                int numElements = 0, numSubstances = 0;
                foreach (var e in elements)
                {
                    var states = e.States;
                    if (states.Count > 0)
                    {
                        Debug.Log($"* {e}");
                        e.RegisterSubstances(substanceList, substanceTable);
                        numElements++;
                        numSubstances += states.Count;
                    }
                    else
                    {
                        Debug.LogWarning($"{e.id} has no states");
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
