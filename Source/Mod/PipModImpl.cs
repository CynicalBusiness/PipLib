using STRINGS;

namespace PipLib.Mod
{
    public class PipModImpl : PipMod
    {
        public override string Name => "PipLib";

        public override string Prefix => "Pip";

        public override void Load()
        {
            base.Load();

            CreateElement("DebugElement")
                .SetUnlocalizedName("Debug Element")
                .AddUnlocalizedDescription($"An internal debugging element for {UI.FormatAsKeyWord("PipLib")}, not intended for normal use")
                .AddState(Element.State.Solid)
                .AddState(Element.State.Liquid)
                .AddState(Element.State.Gas)
                .SetColor(new UnityEngine.Color32(255, 80, 255, 255))
                .AddAttributeModifier(db => db.BuildingAttributes.Decor.Id, 1f, isMultiplier: true);
        }

        public override void Initialize()
        {
            base.Initialize();
            logger.Info("initialize ok!");
        }
    }
}
