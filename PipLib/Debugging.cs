using PipLib.Mod;
using System;
using System.Threading;

namespace PipLib
{
    public static class Debugging
    {

        [PipMod.OnStep(PipMod.Step.Load)]
        private static void Hook ()
        {
            // if (PipLib.Options.doHijackLogger == true)
            // {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                UnityEngine.Application.logMessageReceivedThreaded += OnLogMessageRecievedThreaded;
            // }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("unhandled exception!");

            var ex = (Exception)e.ExceptionObject;
            if (!e.IsTerminating)
            {
                PipLib.Logger.Error("Unhandled exception from {0} on thread #{1}({2})", sender.GetType().FullName, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
                if (ex != null)
                {
                    PipLib.Logger.Log(ex);
                }
                else
                {
                    PipLib.Logger.Error("Null exception, found: {0}", e.ExceptionObject);
                }
            }
        }

        private static void OnLogMessageRecievedThreaded (string message, string stack, UnityEngine.LogType type)
        {
            PipLib.Logger.Debug("LogMessage: {0}({1}) at {2}", message, type.ToString(), stack);
        }

    }
}
