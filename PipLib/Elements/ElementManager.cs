using Klei;
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

        private static List<ElementDef> defs = new List<ElementDef>();

        private static FieldInfo SubstanceTableList = AccessTools.Field(typeof(SubstanceTable), "list");

        /// <summary>
        /// Adds a tag to the given element
        /// </summary>
        /// <param name="element">The element to add the tag to</param>
        /// <param name="tag">The tag to add</param>
        public static void AddTag (Element element, Tag tag)
        {
            var tags = element.oreTags;
            var len = tags.Length;
            Array.Resize(ref tags, len + 1);
            tags[len] = tag;
            element.oreTags = tags;
        }

        /// <summary>
        /// Adds the given tag mappings
        /// </summary>
        /// <param name="tags">The tags to add</param>
        public static void AddTags (Dictionary<Element, Tag> tags)
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
                case Element.State.Vacuum:
                    return substanceTable.GetSubstance(SimHashes.Vacuum).material;
                case Element.State.Gas:
                    return substanceTable.GetSubstance(SimHashes.Oxygen).material;
                case Element.State.Liquid:
                    return substanceTable.GetSubstance(SimHashes.Water).material;
                case Element.State.Solid:
                default:
                    return substanceTable.GetSubstance(SimHashes.Unobtanium).material;
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
        internal static void RegisterSimHashes()
        {
            foreach (var def in defs)
            {
                foreach (var state in def.states.Keys)
                {
                    var id = def.GetStateID(state);
                    var hash = def.GetHash(state);

                    simHashTable.Add(hash, id);
                    simHashReverseTable.Add(id, hash);
                }
            }
        }

        internal static void RegisterSubstances(Hashtable substanceList, SubstanceTable substanceTable)
        {
            foreach (var def in defs)
            {
                foreach (var state in def.states.Keys)
                {
                    var id = def.GetStateID(state);
                    var hash = def.GetHash(state);

                    Logger.Info("add substance: {0} {1}", id, hash);
                    substanceTable.GetList().Add(def.CreateSubstance(state, substanceTable));
                }
            }
        }
        internal static void RegisterAttributes()
        {
            foreach (var def in defs)
            {
                foreach (var state in def.states.Keys)
                {
                    var attrs = def.AddOrGetState(state).attributes;
                    if (attrs.Count > 0)
                    {
                        var element = global::ElementLoader.FindElementByHash(def.GetHash(state));
                        if (element != null)
                        {
                            element.attributeModifiers.AddRange(attrs.ConvertAll(a => a.Invoke(Db.Get())));
                        }
                        else
                        {
                            Debug.LogWarning($"Tried to add attributes to {def.GetStateID(state)}, but no element exists!");
                        }
                    }
                }
            }
        }

        [PipMod.TypeCollector(typeof(IElementConfig))]
        internal static void CollectDefs (Type type)
        {
            var ctor = type.GetConstructor(new Type[]{ });
            if (ctor != null)
            {
                Logger.Debug("Found ElementConfig at {0}", type.FullName);
                var config = (IElementConfig) ctor.Invoke(null);
                defs.Add(config.CreateElementDef());
            }
        }

        internal static int CollectElements (string dir, ref List<ElementLoader.ElementEntry> results)
        {
            if (!Directory.Exists(dir)) return 0;

            var foundElements = new List<string>();
            var files = Array.FindAll(Directory.GetFiles(dir), f => PLUtil.PATTERN_YAML.IsMatch(f));
            foreach (var file in files)
            {
                PipLib.Logger.Debug("loading elements from: {0}", file);
                var elementCollection = YamlIO.Parse<ElementLoader.ElementEntryCollection>(File.ReadAllText(file), Path.GetFileName(file));
                if (elementCollection != null && elementCollection.elements != null)
                {
                    results.AddRange(elementCollection.elements);
                    foundElements.AddRange(Array.ConvertAll(elementCollection.elements, e => e.elementId));
                }
            }

            if (foundElements.Count > 0)
            {
                Logger.Verbose("Loaded {0} element(s) from '{1}': {2}", foundElements.Count, Path.GetDirectoryName(dir), string.Join(",", foundElements.ToArray()));
            }
            return foundElements.Count;
        }
    }
}
