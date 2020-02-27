using Klei;
using Klei.AI;
using PipLib.Mod;
using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PipLib.Elements
{
    public static class ElementManager {
        internal static readonly Logging.ILogger Logger = PipLib.Logger.Fork(nameof(ElementManager));

        internal static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        internal static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        internal static Hashtable substanceList;
        internal static SubstanceTable substanceTable;

        public static List<ElementEntryExtended> loadedElements = new List<ElementEntryExtended>();

        private static FieldInfo SubstanceTableList = AccessTools.Field(typeof(SubstanceTable), "list");

        /// <summary>
        /// Adds tags to the given element
        /// </summary>
        /// <param name="element">The element to add the tag to</param>
        /// <param name="tags">The tags to add</param>
        public static void AddTag (Element element, params Tag[] tags)
        {
            element.oreTags = PLUtil.ArrayConcat(element.oreTags, tags);
        }

        /// <summary>
        /// Adds the given tag mappings
        /// </summary>
        /// <param name="tags">The tags to add</param>
        public static void AddBulkTags (Dictionary<Element, Tag[]> tags)
        {
            foreach (var tagsEntry in tags)
            {
                AddTag(tagsEntry.Key, tagsEntry.Value);
            }
        }

        /// <summary>
        /// Gets the base material for a given <see cref="Element.State"/>
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="substanceTable">The table of vanilla substances</param>
        /// <returns>The material (which can be null)</returns>
        public static Material GetBaseMaterialForState(Element.State state, SubstanceTable substanceTable)
        {
            switch (state)
            {
                case Element.State.Liquid:
                    return substanceTable.liquidMaterial;
                case Element.State.Solid:
                    return substanceTable.solidMaterial;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the default <see cref="KAnimFile"/> for a given <see cref="Element.State"/>.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="substanceTable">The table of vanilla substances</param>
        /// <returns>The anim</returns>
        public static KAnimFile GetDefaultKAnimForState(Element.State state, SubstanceTable substanceTable)
        {
            switch (state)
            {
                case Element.State.Liquid:
                    return substanceTable.GetSubstance(SimHashes.Water).anim;
                case Element.State.Gas:
                    return substanceTable.GetSubstance(SimHashes.Hydrogen).anim;
                default:
                    return substanceTable.GetSubstance(SimHashes.Unobtanium).anim;
            }
        }

        [PipMod.OnStep(PipMod.Step.Load)]
        internal static void Register()
        {
            foreach (var element in loadedElements)
            {
                var id = GetElementID(element);
                var hash = GetElementHash(element);


            }
        }

        /// <summary>
        /// Merges the given element entries into the given source value
        /// </summary>
        /// <param name="source">The source entry to load into</param>
        /// <param name="from">The entries to load from</param>
        public static void MergeElementEntries (ElementLoader.ElementEntry source, params ElementLoader.ElementEntry[] from)
        {
            foreach (var entry in from)
            {
                foreach (var field in entry.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = field.GetValue(entry);
                    if (value != null)
                    {
                        field.SetValue(source, value);
                    }
                }
                foreach (var prop in entry.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetGetMethod().Invoke(entry, new object[0]);
                    if (value != null)
                    {
                        prop.GetSetMethod().Invoke(source, new object[1]{ value });
                    }
                }
            }
        }

        public static Color32 CreateColor32 (ElementEntryExtended.Color color)
        {
            if (color == null) return new Color32(255, 255, 255, 255);
            return new Color32(
                color.r,
                color.g,
                color.b,
                color.a
            );
        }

        public static AttributeModifier CreateAttributeModifier (ElementEntryExtended.Attribute attr)
        {
            if (attr == null) return null;
            return new AttributeModifier(
                attribute_id: attr.attributeId,
                value: attr.value,
                description: attr.description,
                is_multiplier: attr.isMultiplier,
                uiOnly: attr.isUIOnly,
                is_readonly: attr.isReadonly
            );
        }

        /// <summary>
        /// Gets an element's ID from a given entry
        /// </summary>
        /// <param name="entry">The entry to get the ID from</param>
        /// <returns></returns>
        public static string GetElementID (ElementLoader.ElementEntry entry)
        {
            return entry.elementId;
        }

        /// <summary>
        /// Gets a <see cref="SimHashes"/> representing this entry
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <returns></returns>
        public static SimHashes GetElementHash (ElementLoader.ElementEntry entry)
        {
            return GetElementHash(GetElementID(entry));
        }

        public static SimHashes GetElementHash (string id)
        {
            return (SimHashes)Hash.SDBMLower(id);
        }

        /// <summary>
        /// Creates a substance from the given element entry
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <returns></returns>
        public static Substance CreateSubstance (ElementLoader.ElementEntry entry)
        {
            var state = entry.state;
            var id = GetElementID(entry);
            var ext = entry is ElementEntryExtended ? (ElementEntryExtended) entry : null;

            var material = ElementManager.GetBaseMaterialForState(state, substanceTable);
            var materialOverride = ext != null ? ext.material : null;
            if (material == null)
            {
                string materialName = id.ToLower();
                var tex = Assets.GetTexture(materialOverride ?? materialName);
                if (tex != null)
                {
                    material.mainTexture = tex;
                }
                else
                {
                    ElementManager.Logger.Verbose("No material texture '{0}', using default: {1}", materialName, material.mainTexture.name);
                }
                material.name = materialName;
            }

            // get anim
            var animName = ((ext != null ? ext.anim : null) ?? id.ToLower()) + PLUtil.SUFFIX_ANIM;
            KAnimFile animFile = Assets.Anims.Find(a => a.name == animName);
            if (animFile == null)
            {
                animFile = ElementManager.GetDefaultKAnimForState(state, substanceTable);
                if (state == Element.State.Solid) {
                    Logger.Verbose("No anim '{0}' found for {1}, using default: {2}", animName, id, animFile.name);
                }
            }

            // colors
            var color = CreateColor32(ext != null ? ext.color : null);
            var uiColor = CreateColor32(ext != null ? ext.colorUI ?? ext.color : null);
            var conduitColor = CreateColor32(ext != null ? ext.colorUI ?? ext.color : null);

            Logger.Verbose("Created Substance: {0}({1})", id, (int)GetElementHash(entry));
            return ModUtil.CreateSubstance(id, state, animFile, material, color, uiColor, conduitColor);
        }

        internal static void RegisterSubstances()
        {
            Logger.Info("Registering substances...");

            var simHashesLength = Enum.GetValues(typeof(SimHashes)).Length;
            foreach (var element in loadedElements)
            {
                var substance = CreateSubstance(element);
                if (!substanceList.ContainsKey(substance.elementID))
                {
                    var id = element.elementId;
                    var hash = substance.elementID;

                    substanceList.Add(substance.elementID, substance);

                    simHashTable.Add(hash, id);
                    simHashReverseTable.Add(id, hash);
                }
            }

            Logger.Info("Successfully registered {0} substance(s) ({1} total)", loadedElements.Count, loadedElements.Count + simHashesLength);
        }

        internal static void RegisterAttributes ()
        {
            Logger.Info("Registering attributes...");
            foreach (var entry in loadedElements)
            {
                if (entry.attributes != null && entry.attributes.Length > 0)
                {
                    var element = ElementLoader.FindElementByHash(GetElementHash(entry));
                    foreach (var attr in entry.attributes)
                    {
                        element.attributeModifiers.Add(CreateAttributeModifier(attr));
                        Logger.Verbose(" * Applied attribute modifier to {0}: {1} {2}{3} {4}",
                            entry.elementId,
                            attr.attributeId,
                            attr.isMultiplier ? "x" : (attr.value >= 0 ? "+" : ""),
                            attr.value,
                            attr.description);
                    }
                }
            }
        }

        internal static int CollectElements (string dir, List<ElementLoader.ElementEntry> results)
        {
            if (!Directory.Exists(dir)) return 0;

            var foundElements = new List<string>();
            var files = Array.FindAll(Directory.GetFiles(dir), f => PLUtil.PATTERN_YAML.IsMatch(f));
            foreach (var file in files)
            {
                PipLib.Logger.Debug("loading elements from: {0}", file);
                var elementCollection = YamlIO.Parse<ElementEntryExtended.Collection>(File.ReadAllText(file), Klei.FileSystem.FindFileHandle(Path.GetFileName(file)));
                if (elementCollection != null && elementCollection.elements != null)
                {
                    results.AddRange(elementCollection.elements);
                    loadedElements.AddRange(elementCollection.elements);
                    foundElements.AddRange(Array.ConvertAll(elementCollection.elements, e => e.elementId));
                }
            }

            if (foundElements.Count > 0)
            {
                Logger.Info("Loaded {0} element(s) from '{1}': {2}", foundElements.Count, Path.GetDirectoryName(dir), string.Join(",", foundElements.ToArray()));
            }
            return foundElements.Count;
        }
    }
}
