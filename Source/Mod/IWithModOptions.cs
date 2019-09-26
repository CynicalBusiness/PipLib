using System;

namespace PipLib.Mod
{
    public class IWithModOptions<TOptions>
    {

        TOptions Options { get; }
        Type OptionsType { get; }

    }
}
