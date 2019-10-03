using PipLib.Elements;

namespace ExampleMod
{
    // adding elements is similar to adding buildings
    // implement the `IElementConfig` interface and it will be picked up by PipLib
    class ExampleElement : IElementConfig
    {

        public override ElementDef CreateElementDef()
        {
            // we need to create a def for our element, again, much like a building
            var def = ElementTemplates.CreateElementDef("ExampleElement");
            // set the element's color to a pink-ish
            // this can also be overridden on the state-level
            def.color = new UnityEngine.Color32(255, 80, 255, 255);

            // this will add a solid state to our element, called "ExampleElementSolid"
            var solid = def.AddOrGetState(Element.State.Solid);
            // optionally, you can override the defaults for anims in a given state
            solid.anim = "sand_kanim";
            // add an attribute of +50% decor to our solid
            solid.attributes.Add(db => new Klei.AI.AttributeModifier(db.BuildingAttributes.Decor.Id, 0.5f, is_multiplier: true));

            // add a liquid state, using all the defaults
            def.AddOrGetState(Element.State.Liquid);

            return def;
        }

    }
}
