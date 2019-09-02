using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipLib.Mod;
using PipLib.World;
using Klei;

namespace ExampleMod
{
    public class ExampleMod : PipMod
    {

        public const string NAME = "Example";

        public ExampleMod() : base(NAME) { }

        public override void Load()
        {
            // load an asset bundle into the AssetLoader
            LoadAssetBundle("resources.assets");

            // let's make some new elements
            // in this case, tin and it's ore, casserite, as well as bronze
            var tin = new PipElement(this, "Tin") { name = "Tin" }
                .AddSolid() // Add a solid state for our element. This creates a material and KAnim.
                // .AddLiquid()
                // .AddGas()
                .AddBuildingOverheatModifier(-20f) // set a flat -20C overheat modifier, like lead
                .AddBuildingDecorModifier(0.1f); // and let's give it a +10% decor bonus
            var tinOre = new PipElement(this, "TinOre") { name = "Casserite", desc = $"A raw ore that can be refined into {STRINGS.UI.FormatAsLink("Tin", tin.SimId(Element.State.Solid))}." }
                .AddSolid() // same deal here
                .AddBuildingOverheatModifier(-20f);
            var bronze = new PipElement(this, "Bronze") { name = "Bronze", desc = $"A strong alloy formed from {STRINGS.UI.FormatAsLink("Tin", tin.SimId(Element.State.Solid))} and {STRINGS.UI.FormatAsLink("Copper", nameof(STRINGS.ELEMENTS.COPPER))}." }
                .AddSolid()
                .AddBuildingOverheatModifier(75f)
                .AddBuildingDecorModifier(0.2f);
            // we'll also want to define these elements in an `elements.yaml` to set things like material properties and tags.

            // register our new elements with the game
            AddElements(new PipElement[] { tin, tinOre });
        }
    }
}
