namespace PipLib.Mod
{
    /// <summary>
    /// A generic factory class for a given type
    /// </summary>
    public abstract class AbstractFactory
    {
        public readonly PrefixedId id;

        public AbstractFactory(PrefixedId id)
        {
            this.id = id;
        }

        public string Name => id.id;

        public PipMod Mod => id.mod;

        public string Id ()
        {
            return id.mod.Prefix + id.id;
        }
    }
}
