using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using PipLib.Asset;
using PipLib.Mod;
using UnityEngine;

namespace PipLib.World
{
    public class PipElement : PipObject
    {

        public static readonly Color32 WHITE = new Color32(255, 255, 255, 255);

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
        public KAnimFile GetDefaultKAnimForState(Element.State state, SubstanceTable substanceTable)
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

        public static readonly Color32 DEFAULT_COLOR = new Color32(255, 255, 255, 255);

        public struct PipElementState
        {
            public Element.State state;
            public PipElement material;
            public SimHashes simHash;
            public List<Func<AttributeModifier>> attributeModifiers;
            public Color32 color;
            public Color32 colorUi;
            public Color32 colorConduit;
        }

        public string name;
        public string desc;
        public Color32 baseColor = WHITE;

        private readonly Dictionary<Element.State, PipElementState> states = new Dictionary<Element.State, PipElementState>();

        public PipElement(PipMod mod, string id) : base(new PrefixedId(mod, id)) { }

        /// <summary>
        /// Gets the Simulation ID (formatted <c>PrefixElementNameState</c>) for this <see cref="PipElement"/>.
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The Simulation Id</returns>
        public string SimId(Element.State state)
        {
            return $"{id.mod.prefix}{id.id}{state}";
        }

        /// <summary>
        /// Gets the Asset ID (formatted <c>prefix_elementname_state</c>) for this <see cref="PipElement"/>.
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The Asset ID</returns>
        public string AssetId(Element.State state)
        {
            return $"{id.mod.prefix}_{id.id}_{state}".ToLower();
        }

        /// <summary>
        /// Gets the Strings ID (formatted <c>PREFIXELEMENTNAMESTATE</c>) for this <see cref="PipElement"/>.
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The Strings ID</returns>
        public string StringsId(Element.State state)
        {
            return SimId(state).ToUpper();
        }

        public Dictionary<Element.State, PipElementState>.KeyCollection States => states.Keys;

        internal void RegisterSimHashes(Dictionary<SimHashes, string> hashTable, Dictionary<string, object> hashTableReverse)
        {
            foreach (var e in states)
            {
                var state = e.Key;
                var data = e.Value;
                string simId = SimId(state);

                hashTable.Add(data.simHash, simId);
                hashTableReverse.Add(simId, data.simHash);
            }
        }

        internal void RegisterSubstances(Hashtable substanceList, SubstanceTable substanceTable)
        {
            foreach (var e in states)
            {
                var state = e.Key;
                var data = e.Value;
                string simId = SimId(state);
                string assetId = AssetId(state);

                var tex = id.mod.GetAsset<Texture2D>(assetId);
                var baseMaterial = GetBaseMaterialForState(state, substanceTable);
                Material mat = null;
                if (baseMaterial != null)
                {
                    mat = new Material(baseMaterial);
                    if (tex != null)
                    {
                        mat.mainTexture = tex;
                    }
                    mat.name = simId + AssetLoader.SUFFIX_MATERIAL;
                }

                var anim = Assets.GetAnim(assetId + AssetLoader.SUFFIX_ANIM) ?? GetDefaultKAnimForState(state, substanceTable);

                Debug.Log($"** {simId} ({data.simHash})");
                substanceList.Add(data.simHash, ModUtil.CreateSubstance(
                    name: simId,
                    state: state,
                    kanim: anim,
                    material: mat,
                    colour: data.color,
                    ui_colour: data.colorUi,
                    conduit_colour: data.colorConduit
                ));
            }
        }

        internal void RegisterStrings(Element.State state)
        {
            string simId = SimId(state);

            string nameStr = GetStringID(state, "name");
            string descStr = GetStringID(state, "desc");

            Strings.Add(nameStr, name ?? simId);
            Strings.Add(descStr, $"{STRINGS.UI.FormatAsLink(name ?? id.ToString(), simId)} in a {STRINGS.UI.FormatAsLink(state.ToString(), "ELEMENTS" + state.ToString())} state.\n\n{desc ?? ""}".Trim());

            Debug.Log($"Added Strings for {simId} ({Strings.Get(nameStr)}): {Strings.Get(descStr).ToString().Replace("\n", "\\n")}");
        }

        internal void RegisterAttributes()
        {
            foreach (var e in states)
            {
                var data = e.Value;
                if (data.attributeModifiers.Count > 0)
                {
                    var element = ElementLoader.FindElementByHash(data.simHash);
                    if (element != null)
                    {
                        element.attributeModifiers.AddRange(data.attributeModifiers.ConvertAll(a => a.Invoke()));
                    }
                    else
                    {
                        Debug.LogWarning($"Tried to add attributes to {SimId(data.state)}, but no element exists!");
                    }
                }
            }
        }

        public string GetStringID(Element.State state, string suffix)
        {
            return $"STRINGS.ELEMENTS.{StringsId(state)}.{suffix.ToUpper()}";
        }

        public bool TryGetState(Element.State state, out PipElementState materialState)
        {
            return states.TryGetValue(state, out materialState);
        }

        /// <summary>
        /// Adds a new <see cref="Element.State"/> to this <see cref="PipElement"/>.
        /// </summary>
        /// <param name="state">The state to add</param>
        /// <param name="stateColor">The base color of the state. Defaults to white.</param>
        /// <param name="colorUi">The color displayed in the UI. Defaults to the state color.</param>
        /// <param name="colorConduit">The color displayed in pipes. Defaults to the state color.</param>
        /// <returns>This element</returns>
        public PipElement AddState(Element.State state, Color32? stateColor, Color32? colorUi = null, Color32? colorConduit = null)
        {
            if (states.ContainsKey(state))
            {
                Debug.LogWarning($"{id} tried to declare multiple substances for state: {state}");
                return this;
            }

            string simId = SimId(state);

            var color = stateColor ?? baseColor;

            if (colorUi == null)
            {
                colorUi = color;
            }

            if (colorConduit == null)
            {
                colorConduit = color;
            }

            states.Add(state, new PipElementState()
            {
                state = state,
                material = this,
                simHash = (SimHashes)((HashedString)simId).HashValue,
                attributeModifiers = new List<Func<AttributeModifier>>(),
                color = color,
                colorUi = (Color32)colorUi,
                colorConduit = (Color32)colorConduit
            });
            RegisterStrings(state);

            return this;
        }

        public PipElement AddSolid(Color32? stateColor = null)
        {
            return AddState(Element.State.Solid, stateColor);
        }

        public PipElement AddLiquid(Color32? stateColor = null, Color32? colorUi = null, Color32? colorConduit = null)
        {
            return AddState(Element.State.Liquid, stateColor, colorUi, colorConduit);
        }

        public PipElement AddGas(Color32? stateColor = null, Color32? colorUi = null, Color32? colorConduit = null)
        {
            return AddState(Element.State.Gas, stateColor, colorUi, colorConduit);
        }

        /// <summary>
        /// Adds an attriubte to the given <see cref="Element.State"/> of this <see cref="PipElement"/>.
        /// </summary>
        /// <param name="state">The state to add to</param>
        /// <param name="attribute">The attribute to add, wrapped in a function</param>
        /// <returns>This element</returns>
        public PipElement AddAttribute(Element.State state, Func<AttributeModifier> attribute)
        {
            if (TryGetState(state, out var data))
            {
                // this is a function instead of a normal argument, because calling Db.Get() early breaks things
                data.attributeModifiers.Add(attribute);
            }
            else
            {
                Debug.LogWarning($"{id} tried to add attribute to the {state} state before the state was added");
            }
            return this;
        }

        public PipElement AddBuildingDecorModifier(float val, bool isMultiplier = true)
        {
            return AddAttribute(Element.State.Solid, () => new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, val, null, isMultiplier, false));
        }

        public PipElement AddBuildingOverheatModifier(float val, bool isMultiplier = false)
        {
            return AddAttribute(Element.State.Solid, () => new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, val, null, isMultiplier, false));
        }

        public override string ToString()
        {
            return $"{GetType().Name}[{id}:{string.Join(",", new List<Element.State>(States).ConvertAll(s => s.ToString()).ToArray())}]";
        }
    }
}
