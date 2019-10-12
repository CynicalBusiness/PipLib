using System;

namespace PipLib.Options
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Option : Attribute
    {

        public string Name { get; set; }

        public string Tooltip { get; set; }

        public Option (string name)
        {
            Name = name;
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class Selection : Attribute
        {
            public string Name { get; set; }
            public string Tooltip { get; set; }
        }

    }
}
