using STRINGS;

namespace PipLib.Mod.Impl
{
    public class PipModImpl : PipMod
    {

        public override string Name => "PipLib";

        public override string Prefix => "PL";

        public override void Initialize()
        {
            Logger.Info("initialize ok!");
        }
    }
}
