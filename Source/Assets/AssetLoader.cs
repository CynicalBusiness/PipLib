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
        public const string SUFFIX_ANIM = "_kanim";

        /// <summary>
        /// Gets the directory of the assembly the given mod is in
        /// </summary>
        /// <param name="mod">The mod to get</param>
        /// <returns>The fully directory path to the assembly</returns>
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

        /// <summary>
        /// Loads a bundle into this asset loader
        /// </summary>
        /// <param name="mod">The mod that owns the bundle</param>
        /// <param name="bundle">The name of the bundle</param>
        /// <returns>The number of assets loaded</returns>
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

        /// <summary>
        /// Gets an asset from this bundle
        /// </summary>
        /// <typeparam name="T">The type of asset to get</typeparam>
        /// <param name="id">The ID of the asset</param>
        /// <param name="value">The value to assign the asset to</param>
        /// <returns>Whether or not the asset was found</returns>
        public bool GetAsset<T>(PrefixedId id, out T value) where T : UnityEngine.Object
        {
            var assetId = new PrefixedId(id.mod, $"{id.id}.{typeof(T).Name}");
            bool found = assets.TryGetValue(assetId, out var obj);
            value = (T)obj;
            return found;
        }
    }
}
