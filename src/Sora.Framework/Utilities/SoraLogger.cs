using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Console = Colorful.Console;

namespace Sora.Framework.Utilities
{
    public class SoraLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
    }

    public static class LCOL
    {
        public const string WHITE = "%#FFFFFF%";
        public const string BLACK = "%#000000%";
        public const string GRAY = "%#9B9B9B%";
        public const string YELLOW = "%#F1FC5A%";
        public const string PURPLE = "%#B342F4%";
        public const string RED = "%#F94848%";
        public const string DARK_RED = "%#800000%";
        public const string BLUE = "%#1E88E5%";
    }
    
    public class SoraLogger : ILogger
    {
        private readonly string _name;
        private readonly SoraLoggerConfiguration _config;

        public SoraLogger(SoraLoggerConfiguration config)
        {
            _config = config;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            /*
            if (!IsEnabled(logLevel))
            {
                return;
            }
            */
            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                Color prefixColor;
                var prefix = $"";

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        prefixColor = Color.FromArgb(0xCECECE);
                        prefix += $"[TRACE]";
                        break;
                    case LogLevel.Debug:
                        prefixColor = Color.FromArgb(0xCECECE);
                        prefix += $"[DEBUG]";
                        break;
                    case LogLevel.Information:
                        prefixColor = Color.FromArgb(0x37B6FF);
                        prefix += $"[INFO]";
                        break;
                    case LogLevel.Warning:
                        prefixColor = Color.FromArgb(0xFFDA38);
                        prefix += $"[WARNING]";
                        break;
                    case LogLevel.Error:
                        prefixColor = Color.FromArgb(0xFF4C4C);
                        prefix += $"[ERROR]";
                        break;
                    case LogLevel.Critical:
                        prefixColor = Color.FromArgb(0x910000);
                        prefix += $"[CRITICAL]";
                        break;
                    case LogLevel.None:
                        prefixColor = Color.FromArgb(0x000000);
                        prefix = "";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }

                prefix += " ";

                var msg = state.ToString();
                if (exception != null)
                    msg += "\n" + exception;
                
                PrintColorized(prefix, prefixColor, logLevel == LogLevel.Critical, msg);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private static void PrintColorized(string Prefix, Color PrefixColor, bool exit, string msg)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var color = PrefixColor;

            Console.WriteFormatted(Prefix, color);

            color = Color.White;
            foreach (var x in Regex.Split(msg, @"\%(.*?)\%"))
            {
                if (!x.StartsWith("#") || !Hex.IsHex(x.TrimStart('#')) || x.Length != 7)
                {
                    Console.WriteFormatted(x, color);
                    continue;
                }

                color = Color.FromArgb(int.Parse(x.Replace("#", "").ToUpper().Trim(), NumberStyles.HexNumber));
            }

            Console.WriteLine();

            if (exit)
                Environment.Exit(1);
        }
    }

    public class SoraLoggerProvider : ILoggerProvider
    {
        private readonly SoraLoggerConfiguration _config;

        private readonly ConcurrentDictionary<string, SoraLogger> _loggers =
            new ConcurrentDictionary<string, SoraLogger>();

        public SoraLoggerProvider(SoraLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SoraLogger(_config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}