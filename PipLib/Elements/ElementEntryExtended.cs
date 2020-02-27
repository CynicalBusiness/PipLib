
namespace PipLib.Elements
{
    public class ElementEntryExtended : ElementLoader.ElementEntry
    {

        public class Collection {
            public ElementEntryExtended[] elements { get; set; }
        }

        public class Color {
            public byte r { get; set; }

            public byte g { get; set; }

            public byte b { get; set; }

            public byte a { get; set; }
        }

        public class Attribute {
            public string attributeId { get; set; }
            public float value { get; set; }
            public string description { get; set; }
            public bool isMultiplier { get; set; }
            public bool isUIOnly { get; set; }
            public bool isReadonly { get; set; }
        }

        public string material { get; set; }

        public string anim { get; set; }

        public Color color { get; set; }

        public Color colorUI { get; set; }

        public Color colorConduit { get; set; }

        public Attribute[] attributes { get; set; }

    }
}
