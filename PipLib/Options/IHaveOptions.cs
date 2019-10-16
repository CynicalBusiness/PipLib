
namespace PipLib.Options
{
    public interface IHaveOptions<TOptions>
    {
        /// <summary>
        /// The instance of your options class
        /// </summary>
        /// <value></value>
        TOptions Options { get; }

        /// <summary>
        /// The name of the options file. If null, the type name of the options type is used
        /// </summary>
        /// <value></value>
        string OptionsName { get; }
    }

    public interface IHaveOptions : IHaveOptions<object> { }
}
