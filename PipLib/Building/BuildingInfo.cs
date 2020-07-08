using System;

namespace PipLib.Building
{
    public static class BuildingInfo
    {

        [AttributeUsage(AttributeTargets.Class)]
        public abstract class Attr : Attribute
        {
            public string ID { get; private set; }

            public Attr (string id)
            {
                ID = id;
            }
        }

        /// <summary>
        /// Marks a building as requiring a specific tech
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public sealed class TechRequirement : Attr
        {
            public string Tech { get; private set; }

            public TechRequirement (string id, string tech) : base(id)
            {
                Tech = tech;
            }
        }

        /// <summary>
        /// Adds a building to the plan screen
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public sealed class OnPlanScreen : Attr
        {
            public string AfterId { get; set; }

            public int AtIndex { get; set; }

            public string Category { get; private set; }

            public OnPlanScreen (string id, string category) : base(id)
            {
                Category = category;
                AtIndex = -1;
            }
        }

    }
}
