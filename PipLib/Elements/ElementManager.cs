using System;
using System.Collections.Generic;

namespace PipLib.Elements
{

    public static class ElementManager
    {

        /// <summary>
        /// Adds a tag to the given element
        /// </summary>
        /// <param name="element">The element to add the tag to</param>
        /// <param name="tag">The tag to add</param>
        public static void AddTag (Element element, Tag tag)
        {
            var tags = element.oreTags;
            var len = tags.Length;
            Array.Resize(ref tags, len + 1);
            tags[len] = tag;
        }

        /// <summary>
        /// Adds the given tag mappings
        /// </summary>
        /// <param name="tags">The tags to add</param>
        public static void AddTags (Dictionary<Element, Tag> tags)
        {
            foreach (var tagsEntry in tags)
            {
                AddTag(tagsEntry.Key, tagsEntry.Value);
            }
        }
    }

}
