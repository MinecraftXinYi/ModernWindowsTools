﻿using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly object logLock = new();
        private static readonly string logName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string unknown = "unknown";

        private static bool isInitialized = false;
        private static DirectoryInfo logDirectory;
        private static Guid FOLDERID_LocalAppData = new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            Shell32Library.SHGetKnownFolderPath(FOLDERID_LocalAppData, KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, IntPtr.Zero, out string localAppdataPath);

            if (!string.IsNullOrEmpty(localAppdataPath))
            {
                try
                {
                    if (Directory.Exists(localAppdataPath))
                    {
                        string logFolderPath = Path.Combine(localAppdataPath, "Logs");
                        logDirectory = Directory.CreateDirectory(logFolderPath);
                        isInitialized = true;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(EventLevel logLevel, string logContent, StringBuilder logBuilder)
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    try
                    {
                        lock (logLock)
                        {
                            File.AppendAllText(
                                    Path.Combine(logDirectory.FullName, string.Format("{0}_{1}.log", logName, DateTime.Now.ToString("yyyy_MM_dd"))),
                                    string.Format("{0}\t{1}:{2}{3}{4}{5}{6}{7}{8}",
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        "LogType",
                                        Convert.ToString(logLevel),
                                        Environment.NewLine,
                                        "LogContent:",
                                        logContent,
                                        Environment.NewLine,
                                        logBuilder,
                                        Environment.NewLine)
                                    );
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                });
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(EventLevel logLevel, string logContent, Exception exception)
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    try
                    {
                        StringBuilder exceptionBuilder = new();
                        exceptionBuilder.Append("LogContent:");
                        exceptionBuilder.AppendLine(logContent);
                        exceptionBuilder.Append("HelpLink:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("Message:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("HResult:");
                        exceptionBuilder.AppendLine(exception.HResult.ToString());
                        exceptionBuilder.Append("Source:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("StackTrace:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(logDirectory.FullName, string.Format("{0}_{1}.log", logName, DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogType",
                                    Convert.ToString(logLevel),
                                    Environment.NewLine,
                                    exceptionBuilder.ToString(),
                                    Environment.NewLine)
                                );
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                });
            }
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static void OpenLogFolder()
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    Process.Start(logDirectory.FullName);
                });
            }
        }

        /// <summary>
        /// 清除所有的日志文件
        /// </summary>
        public static bool ClearLog()
        {
            try
            {
                Task.Run(() =>
                {
                    string[] logFiles = Directory.GetFiles(logDirectory.FullName, "*.log");
                    foreach (string logFile in logFiles)
                    {
                        File.Delete(logFile);
                    }
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
