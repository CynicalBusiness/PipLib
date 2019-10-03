using PipLib.Mod;
using PipLib.Tech;

namespace ExampleMod
{

    // to create a mod, simply extend PipMod
    public class ExampleMod : PipMod
    {
        // this is the public name of your mod, and is required
        public override string Name => "ExampleMod";

        // optionally, you can provide a prefix for logging and namespacing
        public override string Prefix => "Example";

        public override void Load()
        {
            // occurs after this mod has been loaded (but not necessarily all)
        }

        public override void PostLoad()
        {
            // occurs after all mods have been loaded
        }

        public override void PreInitialize()
        {
            // before database initialization
        }

        public override void Initialize()
        {
            // after database initialization

            // === CUSTOM TECH
            // using PipLib.Tech
            // creating tech happens after the database has been initialized
            // the research tree will be updated automatically
            var tech = TechTree.CreateTech("ExampleTech");
            // add the "farming" tech as a requirement to ours
            // this also assigns the tier to be one greater than farming's tier
            // this can be overridden with `TechTree.SetTier()`
            TechTree.AddRequirement(tech, TechTree.GetTech("FarmingTech"));
            // a tech needs to have "unlockedItems" or an error will appear
            // you can either register a build to this tech now, or add a non-building TechItem
            tech.unlockedItems.Add(Db.Get().TechItems.jetSuit);
            // note that `TechItems.gammaResearchPoint` causes a crash when used
        }

        public override void PostInitialize()
        {
            // after database initialization, all mods initialize, and most patches to Db.Initialize()
        }
    }
}
