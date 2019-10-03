using UnityEngine;

namespace PipLib.Elements
{
    public static class ElementTemplates
    {

        /// <summary>
        /// Creates a new <see cref="ElementDef"/> from the given data
        /// </summary>
        /// <param name="id">The ID of the element</param>
        /// <returns>The created def</returns>
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
