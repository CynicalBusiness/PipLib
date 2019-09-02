using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PipLib.Mod;
using UnityEngine;

namespace PipLib.Asset
{
    /**
     * Helper class for loading and managing assets
     */
    public class AssetLoader
    {

        /** Name of the default resource bundle file */
        public const string RESOURCE_BUNDLE_NAME = "resources.assets";
        public const string ELEMENTS = "elements";

        public const string SUFFIX_MATERIAL = "_mat";
        public const string SUFFIX_ITEM = "_item";

        public const string SUFFIX_BUILD = "_build";
        public const string SUFFIX_ANIM = "_anim";

        public static string GetAssemblyDirectory(PipMod mod)
        {
            return Path.GetDirectoryName(Assembly.GetAssembly(mod.GetType()).Location);
        }

        private static AssetLoader instance;
        private static readonly object _lock = new object();

        public static AssetLoader Get()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new AssetLoader();
                }
                return instance;
            }
        }

        public readonly Dictionary<PrefixedId, UnityEngine.Object> assets = new Dictionary<PrefixedId, UnityEngine.Object>();

        public KAnimFile BuildKAnim(PrefixedId id)
        {
            if (!GetAsset<TextAsset>(new PrefixedId(id.mod, id.id + SUFFIX_ANIM), out var anim))
            {
                throw new KAnimComponentMissingException(id, "anim");
            }
            if (!GetAsset<TextAsset>(new PrefixedId(id.mod, id.id + SUFFIX_BUILD), out var build))
            {
                throw new KAnimComponentMissingException(id, "build");
            }

            int texIndex = 0;
            var textures = new List<Texture2D>();
            while (GetAsset(new PrefixedId(id.mod, $"{id.id}_{texIndex++}"), out Texture2D tex))
            {
                textures.Add(tex);
            }
            var kanim = ModUtil.AddKAnim($"{id.mod.name.ToLower()}_{id.id}", anim, build, textures);
            Debug.Log($"Built KAnimFile '{kanim.name}' with {kanim.textures.Count} texture(s)");

            return kanim;
        }

        /**
         * Loads an asset bundle into this loader
         */
        public int LoadBundle(PipMod mod, string bundle = RESOURCE_BUNDLE_NAME)
        {
            Debug.Log($"Loading bundle '{bundle}' for ${mod}");
            string bundlePath = Path.Combine(GetAssemblyDirectory(mod), bundle);

            int count = 0;

            try
            {
                foreach (var asset in AssetBundle.LoadFromFile(bundlePath).LoadAllAssets())
                {
                    string assetFullName = $"{asset.name}.{asset.GetType().Name}";
                    var assetId = new PrefixedId(mod, assetFullName);
                    Debug.Log($"* {assetId} {++count}");
                    assets.Add(assetId, asset);
                }
            }
            catch (IOException ex)
            {
                Debug.LogWarning($"Failed loading bundle");
                Debug.LogException(ex);
            }

            return count;
        }

        public bool GetAsset<T>(PrefixedId id, out T value) where T : UnityEngine.Object
        {
            var assetId = new PrefixedId(id.mod, $"{id.id}.{typeof(T).Name}");
            bool found = assets.TryGetValue(assetId, out var obj);
            value = (T)obj;
            return found;
        }
    }

    public class KAnimComponentMissingException : Exception
    {
        public KAnimComponentMissingException(PrefixedId animName, string missingComponent) : base($"KAnim ${animName} missing component: {missingComponent}") { }
    }
}
