using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using PipLib.Logging;

namespace PipLib.Options
{

    public static class OptionsManager
    {

        public const string CONFIGFILE = "config.json";

        internal static readonly ILogger Logger = PipLib.Logger.Fork(nameof(OptionsManager));

        /// <summary>
        /// Gets the file path of the config file belonging to the given assembly. In practice, this returns the config
        /// file located in the same directory as the assembly that owns the given options type
        /// </summary>
        /// <typeparam name="TOptions">The type of options to load</typeparam>
        /// <returns>The path to the config file</returns>
        public static string GetConfigFilePath<TOptions> ()
        {
            return Path.Combine(Path.GetDirectoryName(typeof(TOptions).Assembly.Location), CONFIGFILE);
        }

        /// <summary>
        /// Loads options from disk, creating a new empty options type if needed
        /// </summary>
        /// <typeparam name="TOptions">The type of options to load</typeparam>
        /// <returns>The loaded options</returns>
        public static TOptions LoadOptions<TOptions> ()
        {
            TOptions options = default;
            var path = GetConfigFilePath<TOptions>();
            try
            {
                using (var reader = new JsonTextReader(File.OpenText(path)))
                {
                    options = new JsonSerializer{ MaxDepth = 8 }.Deserialize<TOptions>(reader);
                }
                Logger.Info("Successfully loaded options: {0}", typeof(TOptions).FullName);
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException)
            {
                if (!(ex is FileNotFoundException))
                {
                    // if it's something other than not found, log it
                    Logger.Warning("Could not load mod options for {0}, defaults will be used", typeof(TOptions).FullName);
                    Logger.Log(ex, Logging.Logger.LEVEL.WARN);
                }

                var ctor = typeof(TOptions).GetConstructor(new Type[]{ });
                if (ctor != null)
                {
                    options = (TOptions)ctor.Invoke(null);
                    if (ex is FileNotFoundException)
                    {
                        // if it's just simply not there, make one
                        SaveOptions(options);
                    }
                }
                else
                {
                    Logger.Warning("Could not load default options: no valid (accepts no arguments) public constructor found");
                }
            }
            return options;
        }

        /// <summary>
        /// Saves the given options to disk
        /// </summary>
        /// <param name="options">The options to save</param>
        /// <typeparam name="TOptions">The type of options to use</typeparam>
        public static void SaveOptions<TOptions> (TOptions options)
        {
            var path = GetConfigFilePath<TOptions>();
            try
            {
                using (var writer = new JsonTextWriter(File.CreateText(path)))
                {
                    new JsonSerializer{ MaxDepth = 8 }.Serialize(writer, options);
                }
                Logger.Info("Successfully saved options: {0}", typeof(TOptions).FullName);
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException)
            {
                Logger.Error("Failed saving options: {0}", typeof(TOptions).FullName);
                Logger.Log(ex);
            }
        }

    }

}
