using UnityEngine;

namespace PipLib.Elements
{
    public static class ElementTemplates
    {

        public static ElementDef CreateElementDef (
            string id
        )
        {
            var def = ScriptableObject.CreateInstance<ElementDef>();
            def.PrefabID = id;
            def.InitDef();
            def.anim = id.ToLower() + PLUtil.SUFFIX_ANIM;
            def.color = default;
            return def;
        }

    }
}
