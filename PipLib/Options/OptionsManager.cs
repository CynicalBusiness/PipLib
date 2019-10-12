using Newtonsoft.Json;
using Klei;
using PipLib.Logging;
using PipLib.Mod;
using YamlDotNet.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PipLib.Options
{

    public static class OptionsManager
    {

        public const string CONFIGEXT = ".yaml";

        internal static readonly ILogger Logger = PipLib.Logger.Fork(nameof(OptionsManager));

        private static Dictionary<Type, Type> optionsTypes = new Dictionary<Type, Type>();

        private static MethodInfo reflectMergeModOptions = Array.Find(typeof(OptionsManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static), m => m.Name == nameof(MergeModOptions) && m.IsGenericMethodDefinition);

        [PipMod.TypeCollector(typeof(IPipMod))]
        private static void CollectTypes (Type type)
        {
            // find pip mods implementing our interface
            var opts = Array.Find(type.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHaveOptions<>));
            if (opts != null)
            {
                optionsTypes.Add(type, opts.GetGenericArguments()[0]);
            }
        }

        [PipMod.OnStep(PipMod.Step.PostInstanciate)]
        private static void PostInstanciate (IPipMod mod)
        {
            if (optionsTypes.TryGetValue(mod.GetType(), out var options))
            {
                Logger.Info("Loading options for: {0}", mod.Name);
                reflectMergeModOptions.MakeGenericMethod(new []{ options }).Invoke(null, new []{ mod });
            }
        }

        private static void MergeModOptions<TOptions>(IPipMod mod)
        {
            MergeOptions((IHaveOptions<TOptions>) mod);
            SaveOptions((IHaveOptions<TOptions>) mod);
        }

        /// <summary>
        /// Gets the file path of the config file belonging to the given assembly. In practice, this returns the config
        /// file located in the same directory as the assembly that owns the given options type
        /// </summary>
        /// <param name="optionsProvider">The options</param>
        /// <typeparam name="TOptions">The type of options to load</typeparam>
        /// <returns>The path to the config file</returns>
        public static string GetConfigDir<TOptions> (IHaveOptions<TOptions> optionsProvider)
        {
            return Path.Combine(Path.Combine(KMod.Manager.GetDirectory(), PLUtil.DIR_CONFIG), optionsProvider.GetType().Assembly.GetName().Name);
        }

        public static string GetConfigPath<TOptions> (IHaveOptions<TOptions> optionsProvider)
        {
            return Path.Combine(GetConfigDir(optionsProvider), $"{optionsProvider.OptionsName ?? optionsProvider.GetType().Name}.{PLUtil.EXT_YAML}");
        }

        public static void MergeOptions<TOptions> (IHaveOptions<TOptions> optionsProvider)
        {
            var optsLoaded = LoadOptions(optionsProvider);
            var optsTarget = optionsProvider.Options;

            if (optsLoaded == null)
            {
                Logger.Verbose("No options to merge, either defaults were loaded or there was an error.");
                return;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in typeof(TOptions).GetFields(flags))
            {
                if (field.GetCustomAttributes(typeof(Option), true).Length > 0)
                {
                    // assign fields. Nothing special
                    var val = field.GetValue(optsLoaded);
                    if (val != null && !val.Equals(field.GetValue(optsTarget)))
                    {
                        field.SetValue(optsTarget, val);
                    }
                }
            }

            foreach (var prop in typeof(TOptions).GetProperties(flags))
            {
                if (prop.GetCustomAttributes(typeof(Option), false).Length > 0)
                {
                    // properties are a little weirder. We're only going to set them if they have a setter.
                    var setter = prop.GetSetMethod(true);
                    var getter = prop.GetGetMethod(true);
                    if (setter != null && getter != null)
                    {
                        var val = getter.Invoke(optsLoaded, new object[0]);
                        if (val != null && !val.Equals(getter.Invoke(optsTarget, new object[0])))
                        {
                            setter.Invoke(optsTarget, new []{ val });
                        }
                    }
                }
            }

            Logger.Info("Successfully merged options for: {0}", typeof(TOptions).FullName);
        }

        /// <summary>
        /// Loads options from disk, creating a new empty options type if needed
        /// </summary>
        /// <param name="optionsProvider">The options</param>
        /// <typeparam name="TOptions">The type of options to load</typeparam>
        /// <returns>The loaded options</returns>
        public static TOptions LoadOptions<TOptions> (IHaveOptions<TOptions> optionsProvider)
        {
            var file = GetConfigPath(optionsProvider);

            try {
                var text = FileSystem.ConvertToText(FileSystem.ReadBytes(file));
                var deserializer = new DeserializerBuilder().Build();
                return deserializer.Deserialize<TOptions>(File.ReadAllText(file));
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    Logger.Verbose("No options file present, using defaults");
                }
                else
                {
                    Logger.Error("Failed loading options for: {0}", optionsProvider.GetType().FullName);
                    Logger.Log(ex);
                }
                return default;
            }
        }

        /// <summary>
        /// Saves the given options to disk
        /// </summary>
        /// <param name="optionsProvider">The options</param>
        /// <typeparam name="TOptions">The type of options to use</typeparam>
        public static void SaveOptions<TOptions> (IHaveOptions<TOptions> optionsProvider)
        {
            var file = GetConfigPath(optionsProvider);
            Directory.CreateDirectory(Path.GetDirectoryName(file));

            var serializer = new SerializerBuilder();
            serializer.EmitDefaults();
            // TODO type mappings

            using (var sr = new StringReader(serializer.Build().Serialize(optionsProvider.Options)))
            using (var fw = File.CreateText(file))
            {
                fw.AutoFlush = true;

                while (true)
                {
                    var line = sr.ReadLine();
                    if (!line.IsNullOrWhiteSpace())
                    {
                        var colon = line.IndexOf(':');
                        if (colon > 0)
                        {
                            var fieldName = line.Substring(0, colon);
                            var field = typeof(TOptions).GetField(fieldName);
                            var prop = typeof(TOptions).GetProperty(fieldName);
                            if (field != null || prop != null)
                            {
                                var attrs = (Option[])(field != null ? field.GetCustomAttributes(typeof(Option), true) : prop.GetCustomAttributes(typeof(Option), true));
                                if (attrs.Length > 0)
                                {
                                    fw.WriteLine();
                                    fw.WriteLine("# " + attrs[0].Name);
                                    fw.WriteLine("#   " + attrs[0].Tooltip);
                                }
                            }
                        }

                        fw.WriteLine(line);
                    }
                    else break;
                }
            }
        }

    }

}
