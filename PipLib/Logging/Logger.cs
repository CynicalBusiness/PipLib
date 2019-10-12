using PipLib.Options;
using System;
using System.Diagnostics;
using System.Threading;

namespace PipLib.Logging
{
    /// <summary>
    /// Logging interface for PipLib that supports heirarchal logging
    /// </summary>
    /// <remarks>
    /// Certain log levels will only output if the logging level has been set apprpriately.
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        /// The parent logger to this one
        /// </summary>
        ILogger Parent { get; }

        /// <summary>
        /// Creates a new child logger with this logger's prefix plus the given prefix. If the logger to be created
        /// already exists, it is simply returned
        /// </summary>
        /// <param name="prefix">The prefix to create with</param>
        /// <returns>The new logger</returns>
        ILogger Fork(string prefix);

        /// <summary>
        /// Write the given object to the output, using the given prefixes
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="prefix">The prefixes</param>
        /// <param name="obj">The object to write</param>
        /// <param name="context">An optional Unity context</param>
        void Log(Logger.LEVEL level, string[] prefix, object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs a formatted message at the provided level
        /// </summary>
        /// <param name="level">The level to log at</param>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Log(Logger.LEVEL level, string message, params object[] args);

        /// <summary>
        /// Logs an object at the provided level
        /// </summary>
        /// <param name="level">The level to log at</param>
        /// <param name="obj">The object ot log</param>
        /// <param name="context">An optional unity context</param>
        void Log(Logger.LEVEL level, object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs an exception at the given level, defaulting to <see cref="Logger.LEVEL.ERROR"/>
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="level">The level to log at</param>
        void Log(Exception ex, Logger.LEVEL level = Logger.LEVEL.ERROR);

        /// <summary>
        /// Logs a formatted message at the <see cref="Logger.LEVEL.DEBUG"/> level.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Logs an object at the <see cref="Logger.LEVEL.DEBUG"/> level.
        /// </summary>
        /// <param name="obj">The object to log</param>
        /// <param name="context">An optional Unity context</param>
        void Debug(object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs a formatted message at the <see cref="Logger.LEVEL.INFO"/> level.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Logs an object at the <see cref="Logger.LEVEL.INFO"/> level.
        /// </summary>
        /// <param name="obj">The object to log</param>
        /// <param name="context">An optional Unity context</param>
        void Info(object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs a formatted message at the <see cref="Logger.LEVEL.VERB"/> level.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Verbose(string message, params object[] args);

        /// <summary>
        /// Logs an object at the <see cref="Logger.LEVEL.VERB"/> level.
        /// </summary>
        /// <param name="obj">The object to log</param>
        /// <param name="context">An optional Unity context</param>
        void Verbose(object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs a formatted message at the <see cref="Logger.LEVEL.WARN"/> level.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Warning(string message, params object[] args);

        /// <summary>
        /// Logs an object at the <see cref="Logger.LEVEL.WARN"/> level.
        /// </summary>
        /// <param name="obj">The object to log</param>
        /// <param name="context">An optional Unity context</param>
        void Warning(object obj, UnityEngine.Object context = null);

        /// <summary>
        /// Logs a formatted message at the <see cref="Logger.LEVEL.ERROR"/> level.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="args">Args to format with</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Logs an object at the <see cref="Logger.LEVEL.ERROR"/> level.
        /// </summary>
        /// <param name="obj">The object to log</param>
        /// <param name="context">An optional Unity context</param>
        void Error(object obj, UnityEngine.Object context = null);
    }

    /// <summary>
    /// The main <see cref="ILogger"/> implementation.
    /// </summary>
    public class Logger : ILogger
    {
        public enum LEVEL
        {
            /// <summary>
            /// Debugging output. Only shows if the logging level is set to Debug
            /// </summary>
            DEBUG,
            /// <summary>
            /// Verbose output. Only shows if the logging level is set to Verbose or lower.
            /// </summary>
            VERB,
            /// <summary>
            /// General informative output
            /// </summary>
            INFO,
            /// <summary>
            /// A warning message
            /// </summary>
            WARN,
            /// <summary>
            /// An error message
            /// </summary>
            ERROR
        }

        public ILogger Parent { get; private set; }

        public readonly string prefix;

        public Logger(ILogger parent, string prefix)
        {
            Parent = parent;
            this.prefix = prefix;
        }

        public ILogger Fork(string prefix)
        {
            return new Logger(this, prefix);
        }

        public void Log(LEVEL level, string[] prefix, object obj, UnityEngine.Object context = null)
        {
            string[] newPrefix = new string[prefix.Length + 1];
            newPrefix[0] = this.prefix;
            Array.Copy(prefix, 0, newPrefix, 1, prefix.Length);
            Parent.Log(level, newPrefix, obj, context);
        }

        public void Log(LEVEL level, string message, params object[] args)
        {
            Log(level, new string[] { }, string.Format(message, args));
        }

        public void Log(LEVEL level, object obj, UnityEngine.Object context = null)
        {
            Log(level, new string[] { }, obj, context);
        }

        public void Log(Exception ex, LEVEL level = LEVEL.ERROR)
        {
            Parent.Log(ex, level);
        }

        public void Debug(string message, params object[] args)
        {
            Log(LEVEL.DEBUG, message, args);
        }

        public void Debug(object obj, UnityEngine.Object context = null)
        {
            Log(LEVEL.DEBUG, obj, context);
        }

        public void Info(string message, params object[] args)
        {
            Log(LEVEL.INFO, message, args);
        }

        public void Info(object obj, UnityEngine.Object context = null)
        {
            Log(LEVEL.INFO, obj, context);
        }

        public void Verbose(string message, params object[] args)
        {
            Log(LEVEL.VERB, message, args);
        }

        public void Verbose(object obj, UnityEngine.Object context = null)
        {
            Log(LEVEL.VERB, obj, context);
        }

        public void Warning(string message, params object[] args)
        {
            Log(LEVEL.WARN, message, args);
        }

        public void Warning(object obj, UnityEngine.Object context = null)
        {
            Log(LEVEL.WARN, obj, context);
        }

        public void Error(string message, params object[] args)
        {
            Log(LEVEL.ERROR, message, args);
        }

        public void Error(object obj, UnityEngine.Object context = null)
        {
            Log(LEVEL.ERROR, obj, context);
        }
    }

    /// <summary>
    /// The global <see cref="ILogger"/> instance.
    /// </summary>
    public class GlobalLogger : ILogger
    {
        private static GlobalLogger _instance;

        /// <summary>
        /// Gets the global logger instance
        /// </summary>
        /// <returns>The global logger, creating it if it does not exist</returns>
        public static GlobalLogger Get()
        {
            if (_instance == null)
            {
                _instance = new GlobalLogger();
            }
            return _instance;
        }

        private static string GetTimestamp()
        {
            return System.DateTime.UtcNow.ToString("HH:mm:ss.fff");
        }

        public ILogger Parent => this;

        public ILogger Fork(string prefix)
        {
            return new Logger(this, prefix);
        }

        private bool ShouldWrite(Logger.LEVEL level)
        {
            switch (PipLib.Options.Verbosity)
            {
                case LibOptions.LoggingVerbosity.Info:
                    return level >= Logger.LEVEL.INFO;
                case LibOptions.LoggingVerbosity.Verbose:
                    return level >= Logger.LEVEL.VERB;
                case LibOptions.LoggingVerbosity.Debug:
                default:
                    return true;
            }
        }

        private string GetLoggingHead(Logger.LEVEL level)
        {
            return string.Format(PipLib.Options.DoHijackLogger ? "[{0}]@{1} {2,5}:" : "[{0}] [{1}] [{2}]", GetTimestamp(), Thread.CurrentThread.ManagedThreadId, level.ToString());
        }

        private string GetCaller()
        {
            if (PipLib.Options.DoHijackLogger && PipLib.Options.Verbosity >= LibOptions.LoggingVerbosity.Debug)
            {
                var trace = new StackTrace();
                int currentFrame = 1;
                StackFrame frame;
                do
                {
                    frame = trace.GetFrame(currentFrame++);
                } while (typeof(ILogger).IsAssignableFrom(frame.GetMethod().DeclaringType)
                    || typeof(Debug).IsAssignableFrom(frame.GetMethod().DeclaringType)
                    || frame.GetMethod().DeclaringType.FullName.StartsWith(typeof(LoggingPatches).FullName)
                );

                var method = frame.GetMethod();
                return string.Format("[{0}|{1}] ", method.DeclaringType.FullName, method.Name);
            }
            return "";
        }

        private void Write(Logger.LEVEL level, string[] prefix, object[] data)
        {
            if (!ShouldWrite(level))
            {
                return;
            }

            if (prefix.Length > 0)
            {
                Console.WriteLine(string.Format("{0} {1}[{2}] {3}", GetLoggingHead(level), GetCaller(), string.Join(":", prefix), DebugUtil.BuildString(data)));
            }
            else
            {
                Console.WriteLine(string.Format("{0} {1}{2}", GetLoggingHead(level), GetCaller(), DebugUtil.BuildString(data)));
            }
        }

        public void Log(Logger.LEVEL level, string[] prefix, object obj, UnityEngine.Object context = null)
        {
            Write(level, prefix, context != null ? new object[] { context, obj } : new object[] { obj });
        }

        public void Log(Logger.LEVEL level, string message, params object[] args)
        {
            Log(level, new string[] { }, string.Format(message, args));
        }

        public void Log(Logger.LEVEL level, object obj, UnityEngine.Object context = null)
        {
            Log(level, new string[] { }, obj, context);
        }

        public void Log(Exception ex, Logger.LEVEL level = Logger.LEVEL.ERROR)
        {
            Log(level, ex.Message);
            UnityEngine.Debug.LogException(ex);
        }

        public void Debug(string message, params object[] args)
        {
            Log(Logger.LEVEL.DEBUG, message, args);
        }

        public void Debug(object obj, UnityEngine.Object context = null)
        {
            Log(Logger.LEVEL.DEBUG, obj, context);
        }

        public void Info(string message, params object[] args)
        {
            Log(Logger.LEVEL.INFO, message, args);
        }

        public void Info(object obj, UnityEngine.Object context = null)
        {
            Log(Logger.LEVEL.INFO, obj, context);
        }

        public void Verbose(string message, params object[] args)
        {
            Log(Logger.LEVEL.VERB, message, args);
        }

        public void Verbose(object obj, UnityEngine.Object context = null)
        {
            Log(Logger.LEVEL.VERB, obj, context);
        }

        public void Warning(string message, params object[] args)
        {
            Log(Logger.LEVEL.WARN, message, args);
        }

        public void Warning(object obj, UnityEngine.Object context = null)
        {
            Log(Logger.LEVEL.WARN, obj, context);
        }

        public void Error(string message, params object[] args)
        {
            Log(Logger.LEVEL.ERROR, message, args);
        }

        public void Error(object obj, UnityEngine.Object context = null)
        {
            Log(Logger.LEVEL.ERROR, obj, context);
        }
    }
}
