using PipLib.Tech;
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
            // var debugTech = TechTree.CreateTech("DebugTech");
            // TechTree.AddRequirement(debugTech, TechTree.GetTech("PowerRegulation"));
            // debugTech.unlockedItems.Add(Db.Get().TechItems.suitsOverlay);
        }
    }
}
