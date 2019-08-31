using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipLib.Assets;

namespace PipLib.Mod
{
    public abstract class PipMod
    {

        public readonly string name;

        public PipMod(string name)
        {
            this.name = name;
        }

        /**
         * Loads an asset bundle for this mod
         */
        public int LoadAssetBundle (string bundle = AssetLoader.RESOURCE_BUNDLE_NAME)
        {
            return AssetLoader.Get().LoadBundle(this, bundle);
        }

        /**
         * Get an asset that has been loaded by name
         */
        public T GetAsset<T> (string name) where T: UnityEngine.Object
        {
            AssetLoader.Get().assets.TryGetValue(new PrefixedId(this, $"{typeof(T).Name}.{name}"), out var value);
            return value as T;
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

    public class KleiBase : PipMod
    {
        public KleiBase() : base("Klei")
        {
        }
    }
}
