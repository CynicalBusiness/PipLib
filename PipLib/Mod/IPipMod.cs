using System;
using System.Collections.Generic;

namespace PipLib.Mod
{

    public interface IPipMod
    {
        string Name { get; }

        Version Version { get; }

        string Prefix { get; }

        Type OptionsType { get; }

        void Load();
        void PostLoad();

        void PreInitialize();
        void Initialize();
        void PostInitialize();
    }
}
