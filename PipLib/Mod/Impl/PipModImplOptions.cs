
using PipLib.Options;

namespace PipLib.Mod.Impl
{
    public class PipModImplOptions
    {
        [Option("Rebuild Tech Tree", Tooltip = "Whether or not to rebuild the vanilla tech tree. Since this will disable custom techs, only use this if a conflict occurs.")]
        public bool RebuildTechTree { get; set; }

        [Option("Test Option, Please Ignore", Tooltip = "Internal testing option. Has no effect regardless of value.")]
        public string TestOptionPleaseIgnore { get; set; }

        public PipModImplOptions ()
        {
            RebuildTechTree = true;
            TestOptionPleaseIgnore = "Exactly what it says it is";
        }

    }
}
