using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using PipLib.Mod;
using UnityEngine;

namespace PipLib.Assets
{
    /**
     * Helper class for loading and managing assets
     */
    public class AssetLoader
    {

        /** Name of the default resource bundle file */
        public const string RESOURCE_BUNDLE_NAME = "resources.assets";

        public static string GetAssemblyDirectory (PipMod mod)
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

        /**
         * Loads an asset bundle into this loader
         */
        public int LoadBundle (PipMod mod, string bundle = RESOURCE_BUNDLE_NAME)
        {
            Debug.Log($"PipLib: loading assets for ${mod}");
            string bundlePath = Path.Combine(GetAssemblyDirectory(mod), bundle);

            int count = 0;

            try
            {
                foreach (var asset in AssetBundle.LoadFromFile(bundlePath).LoadAllAssets())
                {
                    string assetFullName = $"{asset.GetType().Name}.{asset.name}";
                    Debug.Log($"* {assetFullName} {++count}");
                    assets.Add(new PrefixedId(mod, assetFullName), asset);
                }
            }
            catch (IOException ex)
            {
                Debug.LogWarning($"Failed loading assets!");
                Debug.LogException(ex);
            }

            return count;
        }
    }
}
