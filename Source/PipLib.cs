using System;
using System.Collections.Generic;
using PipLib.Mod;
using PipLib.World;

namespace PipLib
{
    public class PipLib
    {

        public static readonly List<PipMod> mods = new List<PipMod>();

        public static void OnLoad()
        {
            // worried about race conditions here

            mods.Add(BaseMod.instance);
            // TODO find a dynamic way to do this

            Debug.Log($"Found {mods.Count} mod(s)");
            foreach (var m in mods)
            {
                try
                {
                    Debug.Log($"Loading: {m.name} ({m.GetType().FullName})");
                    m.Load();
                }
                catch (Exception err)
                {
                    Debug.LogWarning($"Failed to load {m.GetType().FullName}");
                    Debug.LogException(err);
                }
            }
            WorldPatches.RegisterAll();
        }
    }
}
