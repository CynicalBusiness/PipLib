using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PipLib.Mod
{
    public class DebugBuildingConfig : IBuildingConfig
    {

        public const string ID = "PipLibDebugBuilding";

        public override BuildingDef CreateBuildingDef()
        {
            var def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "switchpower_kanim",
                hitpoints: 1,
                construction_time: 5f,
                construction_mass: TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
                construction_materials: new string[] { "BuildableAny" },
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: TUNING.BUILDINGS.DECOR.NONE,
                noise: TUNING.NOISE_POLLUTION.NONE);

            def.PermittedRotations = PermittedRotations.R360;

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            var occupier = go.AddOrGet<SimCellOccupier>();
            occupier.notifyOnMelt = true;

            go.AddOrGet<Insulator>();

            var hp = go.AddOrGet<BuildingHP>();
            hp.destroyOnDamaged = true;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            
        }

    }
}
