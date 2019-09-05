﻿using System.Collections.Generic;
using PipLib.Asset;
using PipLib.World;

namespace PipLib.Mod
{
    public abstract class PipMod
    {
        public readonly string name;

        protected internal readonly List<PipElement> elements = new List<PipElement>();

        public PipMod(string name)
        {
            this.name = name;
        }

        public abstract void Load();

        public void AddElements(IEnumerable<PipElement> elements)
        {
            foreach (var element in elements)
            {
                this.elements.Add(element);
            }
        }

        // Obsoleted
        /* public void BuildKAnim(string animName)
        {
            AssetLoader.Get().BuildKAnim(new PrefixedId(this, animName));
        } */

        /**
         * Loads an asset bundle for this mod
         */
        public int LoadAssetBundle(string bundle)
        {
            return AssetLoader.Get().LoadBundle(this, bundle);
        }

        /**
         * Get an asset that has been loaded by name
         */
        public T GetAsset<T>(string name) where T : UnityEngine.Object
        {
            AssetLoader.Get().GetAsset<T>(new PrefixedId(this, name), out var asset);
            return asset;
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            return obj is PipMod mod &&
                   name == mod.name;
        }

        public override int GetHashCode()
        {
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
        }
    }

    public class BaseMod : PipMod
    {
        public const string NAME = "Base";

        public const string MISSING_ANIM_NAME = "missinganim";
        public const string MISSING_TEX_NAME = "missingtex";

        public static BaseMod instance;

        public static void OnLoad()
        {
            PipLib.LoadMod(BaseMod.instance);
        }

        static BaseMod()
        {
            instance = new BaseMod();
        }

        public BaseMod() : base(NAME)
        {
        }

        public override void Load()
        {
            // add elements
            AddElements(new PipElement[]
            {
                new PipElement(this, "DebugElement"){
                    name = "Debug Element",
                    desc = "Internal debugging element for PipLib. Not intended for normal gameplay.",
                    baseColor = new UnityEngine.Color32(255, 80, 255, 255)
                }
                    .AddSolid()
                    .AddLiquid()
                    .AddGas()
                    .AddBuildingOverheatModifier(1000f)
                    .AddBuildingDecorModifier(1f)
            });
        }
    }
}
