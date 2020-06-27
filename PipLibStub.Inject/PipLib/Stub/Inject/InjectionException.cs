namespace PipLib.Stub.Inject
{
    [System.Serializable]
    public class InjectionException : System.Exception
    {
        public InjectionException() { }
        public InjectionException(string message) : base(message) { }
        public InjectionException(string message, System.Exception inner) : base(message, inner) { }
        protected InjectionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public InjectionException(string formatMessage, params object[] parameters)
            : base(string.Format(formatMessage, parameters)) { }
    }
}
