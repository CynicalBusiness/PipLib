using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PipLib
{
    public static class PLUtil
    {
        public enum TemperatureUnit
        {
            KELVIN,
            CELSIUS,
            FARENHEIT
        }

        /// <summary>
        /// The directory where element files are loaded from
        /// </summary>
        public const string DIR_ELEMENTS = "elements";

        /// <summary>
        /// The config directory for mods
        /// </summary>
        public const string DIR_CONFIG = "config";

        /// <summary>
        /// YAML file extension
        /// </summary>
        public const string EXT_YAML = "yaml";

        /// <summary>
        /// JSON file extension
        /// </summary>
        public const string EXT_JSON = "json";

        /// <summary>
        /// A regex pattern to find `yaml`/`yml` files
        /// </summary>
        /// <returns></returns>
        public static readonly Regex PATTERN_YAML = new Regex("\\.ya?ml$", RegexOptions.IgnoreCase);

        /// <summary>
        /// A regex pattern to find `json` files
        /// </summary>
        /// <returns></returns>
        public static readonly Regex PATTERN_JSON = new Regex("\\.json$", RegexOptions.IgnoreCase);

        /// <summary>
        /// The suffix used for <see cref="KAnim"/>.
        /// </summary>
        public const string SUFFIX_ANIM = "_kanim";

        /// <summary>
        /// The suffix used for materials
        /// </summary>
        public const string SUFFIX_MATERIAL = "_mat";

        public const float DELTA_KC = 273.15f;
        public const float DELTA_CF = 32f;
        public const float MULTI_CF = 9f / 5f;

        /// <summary>
        /// Converts the given temperature from one unit to another
        /// </summary>
        /// <param name="temp">The temp to convert</param>
        /// <param name="from">The unit to convert from</param>
        /// <param name="to">  The unit to convert to</param>
        /// <returns>The converted unit</returns>
        public static float ConvertTemperature (float temp, TemperatureUnit from, TemperatureUnit to)
        {
            if (from == to) return temp;

            switch (from)
            {
                case TemperatureUnit.KELVIN:
                    switch(to)
                    {
                        case TemperatureUnit.CELSIUS:
                            return temp - DELTA_KC;
                        case TemperatureUnit.FARENHEIT:
                            return (temp - DELTA_KC) * MULTI_CF + DELTA_CF;
                        default:
                            return temp;
                    }
                case TemperatureUnit.CELSIUS:
                    switch(to)
                    {
                        case TemperatureUnit.KELVIN:
                            return temp + DELTA_KC;
                        case TemperatureUnit.FARENHEIT:
                            return temp * MULTI_CF + DELTA_CF;
                        default:
                            return temp;
                    }
                case TemperatureUnit.FARENHEIT:
                    switch(to)
                    {
                        case TemperatureUnit.KELVIN:
                            return (temp - DELTA_CF / MULTI_CF) + DELTA_KC;
                        case TemperatureUnit.CELSIUS:
                            return temp - DELTA_CF / MULTI_CF;
                        default:
                            return temp;
                    }
                default:
                    return temp;
            }
        }

        public static void NOOP ()
        {
            /* no-op */
        }

        /// <summary>
        /// Gets the path to the directory of the assembly that the given type belongs to
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <returns>The directory of the assembly</returns>
        public static string GetAssemblyDir (Type type)
        {
            return Path.GetDirectoryName(type.Assembly.Location);
        }

        /// <summary>
        /// Loads the given file as a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="filename">The path to the file to load (should be a `.png` or `.jpg`)</param>
        /// <returns>The loaded texture</returns>
        public static Texture2D LoadTexture(string filename)
        {
            var data = File.ReadAllBytes(filename);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }

        /// <summary>
        /// Gets the assembly version belonging to caller of this method
        /// </summary>
        /// <returns>The assembly version</returns>
        public static string GetCurrentVersion ()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Gets the assembly name belonging to caller of this method
        /// </summary>
        /// <returns>The assembly name</returns>
        public static string GetCurrentName ()
        {
            return Assembly.GetCallingAssembly().GetName().Name;
        }

        /// <summary>
        /// Concats a variable number of arrays together
        /// </summary>
        /// <param name="firstArray">The first array to start the concat</param>
        /// <param name="arrays">Additional arrays to concat</param>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <returns>The newly concatenated array</returns>
        public static T[] ArrayConcat<T> (T[] firstArray, params T[][] arrays)
        {
            T[] newArray = firstArray;
            if (arrays.Length > 0)
            {
                foreach (var arr in arrays)
                {
                    newArray = newArray.Concat(arr);
                }
            }
            return newArray;
        }

        /// <summary>
        /// Contactinates the array with another, returning a new array comprised of the two
        /// </summary>
        /// <param name="array">The array to contact to</param>
        /// <param name="merge">The arrat to add</param>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <returns>The array type</returns>
        public static T[] Concat<T> (this T[] array, T[] merge)
        {
            var newArray = new T[array.Length + merge.Length];
            array.CopyTo(newArray, 0);
            merge.CopyTo(newArray, array.Length);
            return newArray;
        }
    }
}
