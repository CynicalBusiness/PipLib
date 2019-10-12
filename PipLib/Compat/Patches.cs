using Harmony;
using System;
using System.Diagnostics;

namespace PipLib.Compat
{
    public static class CompatPatches
    {

        [HarmonyPatch(typeof(Console), "WriteLine", new Type[]{ typeof(string) })]
        private static class Patch_Console_WriteLine
        {
            /*
             * this is meant to clean up the really quite nasty logging output from Ony's mods.
             * Also, because she used `Console.WriteLine` rather than `Debug.Log`, it's not
             * captured by Unity correctly and doesn't show in the debugging console.
             */

            public static bool Prepare ()
            {
                // only do this if we're hijacking the logger
                return PipLib.Options.DoHijackLogger;
            }

            public static bool Prefix (string value)
            {
                // Ony creates new logging instances (usually just called "Logger") that directly
                // invoke `Console.WriteLine`. So, move up to stack frames and see if we
                // are called from `Ony.OxygenNotIncluded.<something>.Logger`
                var stack = new StackFrame(2);
                var type = stack.GetMethod().DeclaringType;
                if (type.Name == "Logger" && type.Namespace.StartsWith("Ony.OxygenNotIncluded"))
                {
                    // the logs have 10 characters (a control char plus 9 chars for color code) at the beginning and end
                    // so, we'll strip those out and hand it along to our normal logging method
                    var message = value.Substring(10, value.Length - 20).Trim();
                    Logging.GlobalLogger.Get().Info(message);
                    return false;
                }
                return true;
            }
        }
    }
}
