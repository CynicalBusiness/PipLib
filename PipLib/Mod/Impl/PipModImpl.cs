using PipLib.Options;
using PipLib.Tech;
using STRINGS;

namespace PipLib.Mod.Impl
{
    public class PipModImpl : PipMod, IHaveOptions<PipModImplOptions>
    {

        internal static PipModImpl Instance { get; private set; }

        private PipModImplOptions options = new PipModImplOptions();

        public PipModImplOptions Options => options;

        public string OptionsName => "config";

        public override string Name => "PipLib";

        public override string Prefix => "PL";

        public PipModImpl()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            Logger.Info("initialize ok!");
            // var debugTech = TechTree.CreateTech("DebugTech");
            // TechTree.AddRequirement(debugTech, TechTree.GetTech("PowerRegulation"));
            // debugTech.unlockedItems.Add(Db.Get().TechItems.suitsOverlay);
        }
    }
}
