using System;
namespace PipLib.Stub
{
    public static class PLModStub {

        public const string PIPLIB_TYPENAME = "PipLib.PipLib";


        /// <summary>
        /// Whether or not PipLib is available
        /// </summary>
        /// <value>True if PipLib is available, false otherwise</value>
        public static bool HasPipLib
        {
            get
            {
                return GetPipLibSafe() != null;
            }
        }

        static PLModStub()
        {
            // this should be called when `DLLLoader` starts digging for DLLs with `OnLoad`
            try
            {
                if (!HasPipLib)
                {
                    throw new PipLibMissingException();
                }
            } catch (Exception ex) {
                throw new PipLibMissingException(ex);
            }
        }

        /// <summary>
        /// Searches for and attempts to fetch the `PipLib` type, or `null` if not found
        /// </summary>
        /// <returns>The type, if found, or `null`</returns>
        public static Type GetPipLibSafe ()
        {
            return Type.GetType(PIPLIB_TYPENAME);
        }

        public static void OnLoad ()
        {
            // hook into onload
            Debug.Log("Still runs?");
        }

    }
}
