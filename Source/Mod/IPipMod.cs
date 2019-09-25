using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipLib.Mod
{
    public interface IPipMod
    {
        string Name { get; }

        string Version { get; }

        string Prefix { get; }

        void Load();
        void PostLoad();

        void PreInitialize();
        void Initialize();
        void PostInitialize();
    }
}
