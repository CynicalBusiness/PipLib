﻿using System;

namespace PipLib
{
    public interface ILogger
    {
        ILogger Parent();
        ILogger Fork(string prefix);
        void Log(Logger.LEVEL level, string[] prefix, object obj, UnityEngine.Object context = null);

        void Log(Logger.LEVEL level, string message, params object[] args);
        void Log(Logger.LEVEL level, object obj, UnityEngine.Object context = null);
        void Log(Exception ex, Logger.LEVEL level = Logger.LEVEL.ERROR);

        void Debug(string message, params object[] args);
        void Debug(object obj, UnityEngine.Object context = null);

        void Info(string message, params object[] args);
        void Info(object obj, UnityEngine.Object context = null);

        void Verbose(string message, params object[] args);
        void Verbose(object obj, UnityEngine.Object context = null);

        void Warning(string message, params object[] args);
        void Warning(object obj, UnityEngine.Object context = null);

        void Error(string message, params object[] args);
        void Error(object obj, UnityEngine.Object context = null);
    }

    public class Logger : ILogger
    {
        public enum LEVEL
        {
            DEBUG,
            INFO,
            VERB,
            WARN,
            ERROR
        }

        private readonly ILogger _parent;

        public readonly string prefix;

        public Logger(ILogger parent, string prefix)
        {
            _parent = parent;
            this.prefix = prefix;
        }

        public ILogger Parent()
        {
            return _parent;
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
            _parent.Log(level, newPrefix, obj, context);
        }

        public void Log(LEVEL level, string message, params object[] args)
        {
            Log(level, new string[] { prefix }, string.Format(message, args));
        }

        public void Log(LEVEL level, object obj, UnityEngine.Object context = null)
        {
            Log(level, new string[] { prefix }, obj, context);
        }

        public void Log(Exception ex, LEVEL level = LEVEL.ERROR)
        {
            _parent.Log(ex, level);
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

    public class GlobalLogger : ILogger
    {
        private static GlobalLogger _instance;

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
            return System.DateTime.UtcNow.ToShortTimeString();
        }

        public ILogger Fork(string prefix)
        {
            return new Logger(this, prefix);
        }

        private void Write(Logger.LEVEL level, string[] prefix, object[] data)
        {
            Console.WriteLine(string.Format("{0} {1,5}: [{2}] {3}", GetTimestamp(), level.ToString(), string.Join(":", prefix), DebugUtil.BuildString(data)));
        }

        public void Log(Logger.LEVEL level, string[] prefix, object obj, UnityEngine.Object context = null)
        {
            Write(level, prefix, context != null ? new object[] { context, obj } : new object[] { obj });
        }

        public ILogger Parent()
        {
            return this;
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
