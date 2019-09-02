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

        /**
         * Gets the base engine material for a given substance state
         */
        public static Material GetBaseMaterialForState(Element.State state, SubstanceTable substanceTable)
        {
            switch (state)
            {
                case Element.State.Vacuum:
                    return substanceTable.GetSubstance(SimHashes.Vacuum).material;
                case Element.State.Gas:
                    return substanceTable.GetSubstance(SimHashes.Hydrogen).material;
                case Element.State.Liquid:
                    return substanceTable.GetSubstance(SimHashes.Water).material;
                case Element.State.Solid:
                    return substanceTable.GetSubstance(SimHashes.SandStone).material;
                default:
                    return substanceTable.GetSubstance(SimHashes.Unobtanium).material;
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

        private readonly Dictionary<Element.State, PipElementState> states = new Dictionary<Element.State, PipElementState>();

        public PipElement(PipMod mod, string id) : base(new PrefixedId(mod, id)) { }

        public string SimId(Element.State state)
        {
            return $"{id.mod.name}{id.id}{state}";
        }

        public string AssetId(Element.State state)
        {
            return $"{id.id}_{state}".ToLower();
        }

        public string StringsId(Element.State state)
        {
            return $"{id.mod.name}{id.id}{state}".ToUpper();
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
                var mat = new Material(GetBaseMaterialForState(state, substanceTable));
                if (tex != null)
                {
                    mat.mainTexture = tex;
                }
                else
                {
                    Debug.LogWarning($"Missing Texture: {assetId} (was the bundle containing it loaded?)");
                    mat.mainTexture = BaseMod.instance.GetAsset<Texture2D>($"{BaseMod.MISSING_TEX_NAME}_{state.ToString().ToLower()}");
                }
                mat.name = $"mat{simId}";

                var anim = Assets.GetAnim(assetId + AssetLoader.SUFFIX_ITEM);
                if (anim == null)
                {
                    Debug.LogWarning($"Missing KAnim: {assetId} ({simId}) (was the bundle containing it loaded? has the anim been built?)");
                    anim = Assets.GetAnim(BaseMod.NAME + "_" + BaseMod.MISSING_ANIM_NAME + AssetLoader.SUFFIX_ITEM);
                }

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
            Strings.Add(descStr, $"{STRINGS.UI.FormatAsLink(name ?? id.ToString(), simId)} in a {STRINGS.UI.FormatAsLink(state.ToString(), "ELEMENTS"+state.ToString())} state.\n\n{desc ?? ""}".Trim());

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

        public PipElement AddState(Element.State state, Color32 color, Color32? colorUi = null, Color32? colorConduit = null)
        {
            if (states.ContainsKey(state))
            {
                Debug.LogWarning($"{this.id} tried to declare multiple substances for state: {state}");
                return this;
            }

            string simId = SimId(state);
            string assetId = AssetId(state);

            if (colorUi == null)
            {
                colorUi = color;
            }

            if (colorConduit == null)
            {
                colorConduit = color;
            }

            try
            {
                id.mod.BuildKAnim(assetId + AssetLoader.SUFFIX_ITEM);
            }
            catch (KAnimComponentMissingException ex)
            {
                Debug.LogWarning($"Failed loading {simId} KAnim");
                Debug.LogException(ex);
            }
            states.Add(state, new PipElementState()
            {
                state = state,
                material = this,
                simHash = (SimHashes)Hash.SDBMLower(simId),
                attributeModifiers = new List<Func<AttributeModifier>>(),
                color = color,
                colorUi = (Color32)colorUi,
                colorConduit = (Color32)colorConduit
            });
            RegisterStrings(state);

            return this;
        }

        public PipElement AddSolid(Color32? color = null)
        {
            return AddState(Element.State.Solid, color == null ? DEFAULT_COLOR : (Color32)color);
        }

        // TODO liquids and gasses

        // this is a function instead of a normal argument, because calling Db.Get() early breaks things
        public PipElement AddAttribute(Element.State state, Func<AttributeModifier> attribute)
        {
            if (TryGetState(state, out var data))
            {
                data.attributeModifiers.Add(attribute);
            }
            else
            {
                Debug.LogWarning($"{this.id} tried to add attribute to the {state} state before the state was added");
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
