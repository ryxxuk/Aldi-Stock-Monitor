﻿using System;
using System.IO;

namespace Aldi_Monitor.Functions
{
    public class LoggingService
    {
        private readonly string _logDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "logs");

        private static LoggingService _loggingServiceSingleton;

        private static object _MessageLock = new object();

        private static LoggingService LoggingServiceSingleton => _loggingServiceSingleton ??= new LoggingService();

        public StreamWriter  sw { get; set; }

        public LoggingService()
        {
            EnsureLogDirectoryExists();
            InstantiateStreamWriter();
        }

        ~LoggingService()
        {
            if (sw == null) return;
            try
            {
                sw.Dispose();
            }
            catch (ObjectDisposedException) { } // object already disposed - ignore exception
        }

        public static void WriteLine(string str)
        {
            lock (_MessageLock)
            {
                Console.Write($"[{DateTime.Now}] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[ALDI] ");
                Console.ResetColor();

                Console.WriteLine(str);

                str = $"[{DateTime.Now}] [ALDI] {str}";
                LoggingServiceSingleton.sw.WriteLine("\n" + str);
            }
        }

        public static void Write(string str)
        {
            lock (_MessageLock)
            {
                Console.Write(str);
                LoggingServiceSingleton.sw.Write(str);
            }
        }

        private void InstantiateStreamWriter()
        {
            var filePath = Path.Combine(_logDirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")) + ".txt";
            try
            {
                sw = new StreamWriter(filePath)
                {
                    AutoFlush = true
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ApplicationException(
                    $"Access denied. Could not instantiate StreamWriter using path: {filePath}.", ex);
            }
        }

        private void EnsureLogDirectoryExists()
        {
            if (Directory.Exists(_logDirPath)) return;

            try
            {
                Directory.CreateDirectory(_logDirPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ApplicationException(
                    $"Access denied. Could not create log directory using path: {_logDirPath}.", ex);
            }
        }
    }
}
