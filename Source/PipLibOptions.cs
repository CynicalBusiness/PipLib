using Newtonsoft.Json;
using PeterHan.PLib;

namespace PipLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PipLibOptions
    {

        [Option("Replace Default Logger", "Whether or not the default logger should be completely replaced.")]
        [JsonProperty]
        public bool doHijackLogger { get; set; }

        [Option("Logging Verbosity", "The verbosity at which logs are written")]
        [JsonProperty]
        public LoggingVerbosity loggingVerbosity { get; set; }

        [Option("Enable Developer Console", "Whether or not to enable the Unity developer console")]
        [JsonProperty]
        public bool enableDeveloperConsole { get; set; }

        public PipLibOptions()
        {
            doHijackLogger = true;
            loggingVerbosity = LoggingVerbosity.Info;
            enableDeveloperConsole = false;
        }

        public override string ToString()
        {
            return string.Format("PipLibOptions[doHijackLogger={0},loggingVerbosity={1},enableDeveloperConsole={2]]", doHijackLogger, loggingVerbosity, enableDeveloperConsole);
        }

        public enum LoggingVerbosity
        {
            [Option("Informational", "The least amount of output: only INFO, WARN, and ERR")]
            Info,
            [Option("Verbose", "Inclues verbose output (i.e. VERB) in additional to all informational output")]
            Verbose,
            [Option("Debugging", "Output will be extremely verbose and output that is not normally included is printed")]
            Debug
        }
    }
}
