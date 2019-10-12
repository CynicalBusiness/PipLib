
namespace PipLib.Options
{
    public class LibOptions
    {
        public class Provider : IHaveOptions<LibOptions>
        {
            public LibOptions Options => new LibOptions();
            public string OptionsName => "lib";
        }

        [Option("Replace Default Logger", Tooltip = "Whether or not the default logger should be completely replaced.")]
        public bool DoHijackLogger { get; set; }

        [Option("Enable Developer Console", Tooltip = "Whether or not to enable the Unity developer console")]
        public bool EnableDeveloperConsole { get; set; }

        [Option("Logging Verbosity", Tooltip = "The verbosity at which logs are written")]
        public LoggingVerbosity Verbosity { get; set; }

        public LibOptions()
        {
            DoHijackLogger = true;
            Verbosity = LoggingVerbosity.Info;
            EnableDeveloperConsole = false;
        }

        public override string ToString()
        {
            return string.Format("PipLibOptions[DoHijackLogger={0},LoggingVerbosity={1},EnableDeveloperConsole={2}]", DoHijackLogger, Verbosity, EnableDeveloperConsole);
        }

        public enum LoggingVerbosity
        {
            [Option.Selection(Name = "Informational", Tooltip = "The least amount of output: only INFO, WARN, and ERR")]
            Info,

            [Option.Selection(Name = "Verbose", Tooltip = "Inclues verbose output (i.e. VERB) in additional to all informational output")]
            Verbose,

            [Option.Selection(Name = "Debugging", Tooltip = "Output will be extremely verbose and output that is not normally included is printed")]
            Debug
        }
    }
}
