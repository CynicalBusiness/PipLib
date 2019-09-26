using System;
using Harmony;

namespace PipLib.Patches
{
    public class Logging
    {

        private static readonly ILogger Logger = GlobalLogger.Get();

        [HarmonyPatch(typeof(Debug), "Log", new Type[] { typeof(object) })]
        private static class Patch_Debug_Log
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj)
            {
                Logger.Info(obj);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "Log", new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        private static class Patch_Debug_Log_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj, UnityEngine.Object context)
            {
                Logger.Info(obj, context);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogFormat", new Type[] { typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogFormat
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(string format, object[] args)
            {
                Logger.Info(format, args);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogFormat", new Type[] { typeof(UnityEngine.Object), typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogFormat_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(UnityEngine.Object context, string format, object[] args)
            {
                Logger.Info((object)string.Format(format, args), context);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogWarning", new Type[] { typeof(object) })]
        private static class Patch_Debug_LogWarning
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj)
            {
                Logger.Warning(obj);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogWarning", new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        private static class Patch_Debug_LogWarning_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj, UnityEngine.Object context)
            {
                Logger.Warning(obj, context);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogWarningFormat", new Type[] { typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogWarningFormat
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(string format, object[] args)
            {
                Logger.Warning(format, args);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogWarningFormat", new Type[] { typeof(UnityEngine.Object), typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogWarningFormat_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(UnityEngine.Object context, string format, object[] args)
            {
                Logger.Warning((object)string.Format(format, args), context);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogError", new Type[] { typeof(object) })]
        private static class Patch_Debug_LogError
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj)
            {
                Logger.Error(obj);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogError", new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        private static class Patch_Debug_LogError_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(object obj, UnityEngine.Object context)
            {
                Logger.Error(obj, context);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogErrorFormat", new Type[] { typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogErrorFormat
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(string format, object[] args)
            {
                Logger.Error(format, args);
                return false;
            }
        }

        [HarmonyPatch(typeof(Debug), "LogErrorFormat", new Type[] { typeof(UnityEngine.Object), typeof(string), typeof(object[]) })]
        private static class Patch_Debug_LogErrorFormat_Contextified
        {
            public static bool Prepare()
            {
                return PipLib.Options.doHijackLogger;
            }

            public static bool Prefix(UnityEngine.Object context, string format, object[] args)
            {
                Logger.Error((object)string.Format(format, args), context);
                return false;
            }
        }

    }
}
