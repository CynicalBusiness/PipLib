using System.Collections.Generic;

namespace PipLib.Mod
{
    public class PrefixedId
    {

        public readonly PipMod mod;
        public readonly string id;

        public readonly string value;

        public PrefixedId(PipMod mod, string id)
        {
            this.mod = mod;
            this.id = id;

            value = $"{mod.name}.{id}";
        }

        public PrefixedId(PipMod mod, PrefixedId id) : this(mod, id.value) { }

        public override string ToString()
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            return obj is PrefixedId id &&
                   EqualityComparer<PipMod>.Default.Equals(mod, id.mod) &&
                   this.id == id.id;
        }

        public override int GetHashCode()
        {
            var hashCode = 404570331;
            hashCode = hashCode * -1521134295 + EqualityComparer<PipMod>.Default.GetHashCode(mod);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            return hashCode;
        }
    }

    public abstract class PipObject
    {
        public readonly PrefixedId id;

        public PipObject(PrefixedId id)
        {
            this.id = id;
        }
    }
}
