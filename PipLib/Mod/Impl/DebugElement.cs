using Klei.AI;
using PipLib.Elements;

namespace PipLib.Mod.Impl
{
    public class DebugElement : IElementConfig
    {
        public const string ID = "DebugElement";

        public override ElementDef CreateElementDef ()
        {
            var def = ElementTemplates.CreateElementDef(ID);
            def.color = new UnityEngine.Color32(255, 83, 255, 255);

            var solid = def.AddOrGetState(Element.State.Solid);
            solid.anim = "sand_kanim";
            solid.attributes.Add(db => new AttributeModifier(db.BuildingAttributes.Decor.Id, 0.5f, "Debug decor value", true));
            def.AddOrGetState(Element.State.Liquid);

            return def;
        }

    }
}
