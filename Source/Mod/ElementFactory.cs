using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace PipLib.Mod
{

    /// <summary>
    /// Factory for creating elements within PipLib.
    /// </summary>
    public class ElementFactory : AbstractFactory
    {

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

        public struct State
        {
            public List<Func<AttributeModifier>> attributeModifiers;
            public HashedString simHash;
        }

        private string name = default;
        private string description = default;
        private Color32 color = default;

        private readonly Dictionary<Element.State, State> states = new Dictionary<Element.State, State>();

        public ElementFactory(PrefixedId id) : base(id) { }

        public string Id(Element.State state)
        {
            return Id() + state.ToString();
        }
        internal void RegisterSimHashes(Dictionary<SimHashes, string> hashTable, Dictionary<string, object> hashTableReverse)
        {
            foreach (var e in states)
            {
                var state = e.Key;
                var data = e.Value;
                string simId = Id(state);

                hashTable.Add((SimHashes)data.simHash.HashValue, simId);
                hashTableReverse.Add(simId, (SimHashes)data.simHash.HashValue);
            }
        }

        internal void RegisterSubstances(Hashtable substanceList, SubstanceTable substanceTable)
        {
            foreach (var e in states)
            {
                var state = e.Key;
                var simHash = e.Value.simHash;
                id.mod.logger.Info("add substance: {0} {1}", Id(state), simHash.HashValue);
                substanceList.Add((SimHashes)simHash.HashValue, Create(state, substanceTable));
            }
        }

        internal void RegisterStrings()
        {
            foreach (var state in states.Keys)
            {
                string simId = Id(state);
                string stringId = "STRINGS.ELEMENTS." + simId.ToUpper();

                string strName = stringId + ".NAME";
                string strDesc = stringId + ".DESC";

                Strings.Add(strName, name ?? simId);
                Strings.Add(strDesc, $"{STRINGS.UI.FormatAsLink(name ?? id.ToString(), simId)} in a {STRINGS.UI.FormatAsLink(state.ToString(), "ELEMENTS" + state.ToString())} state.\n{description ?? ""}".Trim());

                id.mod.logger.Info("add Substance strings: {0} ({1}): {2}", Strings.Get(strName), simId, Strings.Get(strDesc).String.Replace("\n", "\\n"));
            }
        }

        internal void RegisterAttributes()
        {
            foreach (var e in states)
            {
                var state = e.Key;
                var data = e.Value;
                if (data.attributeModifiers.Count > 0)
                {
                    var element = ElementLoader.FindElementByHash((SimHashes)data.simHash.HashValue);
                    if (element != null)
                    {
                        element.attributeModifiers.AddRange(data.attributeModifiers.ConvertAll(a => a.Invoke()));
                    }
                    else
                    {
                        Debug.LogWarning($"Tried to add attributes to {Id(state)}, but no element exists!");
                    }
                }
            }
        }

        public ElementFactory SetUnlocalizedName(string name)
        {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Sets the substance color of this element
        /// </summary>
        /// <param name="color">The color to set to</param>
        /// <returns>This</returns>
        public ElementFactory SetColor(Color32 color)
        {
            this.color = color;
            return this;
        }

        /// <summary>
        /// Appends to the default description of this element
        /// </summary>
        /// <param name="desc">The description to append</param>
        /// <returns>This</returns>
        public ElementFactory AddUnlocalizedDescription(string desc)
        {
            description += "\n" + desc;
            return this;
        }

        /// <summary>
        /// Adds an <see cref="Element.State"/> to this element.
        /// </summary>
        /// <param name="state">The state to add</param>
        /// <returns>This</returns>
        public ElementFactory AddState(Element.State state)
        {
            states.Add(state, new State()
            {
                attributeModifiers = new List<Func<AttributeModifier>>(),
                simHash = new HashedString(Id(state))
            });
            return this;
        }

        /// <summary>
        /// Adds an attribute to this element for the given <see cref="Element.State"/>.
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        /// <param name="state">The state</param>
        /// <returns>This</returns>
        public ElementFactory AddAttribute(Func<AttributeModifier> attribute, Element.State state = Element.State.Solid)
        {
            if (states.TryGetValue(state, out var stateData))
            {
                stateData.attributeModifiers.Add(attribute);
            }
            else
            {
                throw new NotImplementedException("Tried to add attribute to a state that did not exist");
            }
            return this;
        }

        public ElementFactory AddAttributeModifier(Func<Db, string> id, float value, Func<string> desc = null, bool isMultiplier = false, bool uiOnly = false)
        {
            return AddAttribute(() => new AttributeModifier(id.Invoke(Db.Get()), value, desc, isMultiplier, uiOnly));
        }

        public Substance Create(Element.State state, SubstanceTable substanceTable)
        {
            var simId = Id(state);

            var material = GetBaseMaterialForState(state, substanceTable);
            if (material != null)
            {
                string materialName = simId.ToLower() + AssetLoader.SUFFIX_MATERIAL;
                var tex = AssetLoader.GetTexture(id.mod, materialName);
                if (tex != null)
                {
                    material.mainTexture = tex;
                }
                material.name = materialName;
            }

            return ModUtil.CreateSubstance(
                name: simId,
                state: state,
                kanim: Assets.GetAnim(simId.ToLower() + AssetLoader.SUFFIX_ANIM) ?? GetDefaultKAnimForState(state, substanceTable),
                material: material,
                colour: color,
                ui_colour: color,
                conduit_colour: color
            );
        }
    }
}
