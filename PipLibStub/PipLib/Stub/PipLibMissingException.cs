using System.Reflection;
using System;

namespace PipLib.Stub {
    [Serializable]
    public class PipLibMissingException : Exception
    {
        public const string MESSAGE = "PipLib is missing or failed to load but was required at: ";

        private static string GetMessage()
        {
            return MESSAGE + Assembly.GetExecutingAssembly().FullName;
        }

        public PipLibMissingException(): base(GetMessage()) { }
        public PipLibMissingException(Exception inner) : base(GetMessage(), inner) { }

    }
}
