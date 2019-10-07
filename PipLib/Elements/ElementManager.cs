using System;
using System.Collections.Generic;
using PipLib.Logging;

namespace PipLib.Elements
{

    public static class ElementManager
    {

        internal static ILogger Logger = PipLib.Logger.Fork(nameof(ElementManager));

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
            element.oreTags = tags;
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
