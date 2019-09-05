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

        }

        public static void LoadMod(PipMod mod)
        {
            try
            {
                Debug.Log($"Loading: {mod.name} ({mod.GetType().FullName})");
                mod.Load();
                WorldPatches.RegisterAll(mod);
                mods.Add(mod);
            }
            catch (Exception err)
            {
                Debug.LogWarning($"Failed to load {mod.GetType().FullName}");
                Debug.LogException(err);
            }
        }
    }
}
