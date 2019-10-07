using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PipLib.Elements
{
    /// <summary>
    /// <see cref="Def"/> for a new element
    /// </summary>
    public class ElementDef : Def
    {
        public sealed class StateData
        {
            /// <summary>
            /// A reference to the <see cref="ElementDef"/> that owns this state
            /// </summary>
            public readonly ElementDef Def;

            /// <summary>
            /// Attribute functions
            /// </summary>
            public readonly List<Func<Db, AttributeModifier>> attributes = new List<Func<Db, AttributeModifier>>();

            /// <summary>
            /// Animation name override
            /// </summary>
            public string anim;

            /// <summary>
            /// The name of the in-world material
            /// </summary>
            public string material;

            public StateData(ElementDef def)
            {
                Def = def;
            }
        }

        /// <summary>
        /// The default color for the element
        /// </summary>
        public Color32 color;

        /// <summary>
        /// The default color the element uses in the UI (i.e. the material overlay)
        /// </summary>
        public Color32 uiColor;

        /// <summary>
        /// The default color the element uses in pipes
        /// </summary>
        public Color32 conduitColor;

        public readonly Dictionary<Element.State, StateData> states = new Dictionary<Element.State, StateData>();

        /// <summary>
        /// Get a state of this element, adding it if not already present
        /// </summary>
        /// <param name="state">The state to get</param>
        /// <returns>The state</returns>
        public StateData AddOrGetState (Element.State state)
        {
            var found = states.TryGetValue(state, out var stateData);
            if (!found)
            {
                stateData = new StateData(this);
                states.Add(state, stateData);
            }
            return stateData;
        }

        /// <summary>
        /// Gets the in-game ID for a given state
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The ID</returns>
        public string GetStateID (Element.State state)
        {
            return PrefabID + state.ToString();
        }

        /// <summary>
        /// Gets the <see cref="SimHashes"/> for a given state
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The hash</returns>
        public SimHashes GetHash (Element.State state)
        {
            return (SimHashes)Hash.SDBMLower(GetStateID(state));
        }

        /// <summary>
        /// Creates a stubstance from the given state, using defaults from the given substance table
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="substanceTable">The substance table</param>
        /// <returns>The created substance</returns>
        internal Substance CreateSubstance (Element.State state, SubstanceTable substanceTable)
        {
            var simId = GetStateID(state);
            var data = AddOrGetState(state);

            // get material
            var material = ElementLoader.GetBaseMaterialForState(state, substanceTable);
            if (material != null)
            {
                string materialName = data.material ?? simId.ToLower();
                var tex = Assets.GetTexture(materialName);
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
            var animName = data.anim ?? GetAnim(state);
            KAnimFile animFile = Assets.Anims.Find(a => a.name == animName);
            if (animFile == null)
            {
                animFile = ElementLoader.GetDefaultKAnimForState(state, substanceTable);
                ElementManager.Logger.Verbose("No anim '{0}' found, using default: {1}", animName, animFile.name);
            }

            return ModUtil.CreateSubstance(
                name: simId,
                state: state,
                kanim: animFile,
                material: material,
                colour: color,
                ui_colour: color,
                conduit_colour: color
            );
        }

        internal protected string GetAnim (Element.State state)
        {
            return GetStateID(state).ToLower() + PLUtil.SUFFIX_ANIM;
        }
    }
}
