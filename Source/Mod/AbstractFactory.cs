namespace PipLib.Mod
{
    /// <summary>
    /// A generic factory class for a given type
    /// </summary>
    /// <typeparam name="R">The resulting object</typeparam>
    public abstract class AbstractFactory
    {
        public readonly PrefixedId id;

        public AbstractFactory(PrefixedId id)
        {
            this.id = id;
        }

        public string Name => id.id;

        public PipMod Mod => id.mod;
    }
}
