using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipLib.Elements
{
    internal static class ElementLoader {
        private static readonly Logging.ILogger Logger = PipLib.Logger.Fork(nameof(ElementLoader));

        internal static Dictionary<SimHashes, string> simHashTable = new Dictionary<SimHashes, string>();
        internal static Dictionary<string, object> simHashReverseTable = new Dictionary<string, object>();

        private static List<ElementDef> defs = new List<ElementDef>();

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

        internal static void GetDefs (IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (typeof(IElementConfig).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    var ctor = type.GetConstructor(new Type[]{ });
                    if (ctor != null)
                    {
                        Logger.Debug("Found ElementConfig at {0}", type.FullName);
                        var config = (IElementConfig) ctor.Invoke(null);
                        defs.Add(config.CreateElementDef());
                    }
                }
            }
        }

        internal static void RegisterSimHashes(Dictionary<SimHashes, string> hashTable, Dictionary<string, object> hashTableReverse)
        {
            foreach (var def in defs)
            {
                foreach (var state in def.states.Keys)
                {
                    var id = def.GetStateID(state);
                    var hash = def.GetHash(state);

                    hashTable.Add(hash, id);
                    hashTableReverse.Add(id, hash);
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
                    substanceList.Add(hash, def.CreateSubstance(state, substanceTable));
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
    }
}
