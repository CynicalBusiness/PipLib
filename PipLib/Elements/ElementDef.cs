using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PipLib.Elements
{
    public class ElementDef : Def
    {
        public sealed class StateData
        {
            public readonly ElementDef Def;

            public readonly List<Func<Db, AttributeModifier>> attributes = new List<Func<Db, AttributeModifier>>();
            public string anim;

            public StateData(ElementDef def)
            {
                Def = def;
            }
        }

        public Color32 color;
        public Color32 uiColor;
        public Color32 conduitColor;
        public string anim;

        public readonly Dictionary<Element.State, StateData> states = new Dictionary<Element.State, StateData>();

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

        public string GetStateID (Element.State state)
        {
            return PrefabID + state.ToString();
        }

        public SimHashes GetHash (Element.State state)
        {
            return (SimHashes)Hash.SDBMLower(GetStateID(state));
        }

        public Substance CreateSubstance (Element.State state, SubstanceTable substanceTable)
        {
            var simId = GetStateID(state);

            var material = ElementLoader.GetBaseMaterialForState(state, substanceTable);
            if (material != null)
            {
                string materialName = simId.ToLower();
                var tex = Assets.GetTexture(materialName);
                if (tex != null)
                {
                    material.mainTexture = tex;
                }
                material.name = materialName;
            }

            var data = AddOrGetState(state);
            var animName = data.anim ?? anim;
            KAnimFile animFile = Assets.Anims.Find(a => a.name == animName);
            if (animFile == null)
            {
                animFile = ElementLoader.GetDefaultKAnimForState(state, substanceTable);
                PipLib.Logger.Verbose("No anim '{0}' found, using default: {1}", animName, animFile.name);
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
    }
}
