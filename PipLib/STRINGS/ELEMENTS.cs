using STRINGS;

namespace PipLib.STRINGS
{
    public static class ELEMENTS
    {
        public static class DEBUGELEMENTSOLID
        {
            public static readonly LocString NAME = UI.FormatAsLink("Debug Element", nameof(DEBUGELEMENTSOLID));
            public static readonly LocString DESC = "Internal debugging element for PipLib, not intended for normal use.";
        }

        public static class DEBUGELEMENTLIQUID
        {
            public static readonly LocString NAME = UI.FormatAsLink("Debug Element", nameof(DEBUGELEMENTLIQUID));
            public static readonly LocString DESC = DEBUGELEMENTSOLID.DESC;
        }
    }
}
