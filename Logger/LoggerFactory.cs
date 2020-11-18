using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerUtils
{
	public class LoggerFactory
	{
		private static FileLoggerConfiguration fileLoggerConfiguration = new FileLoggerConfiguration();
		private static ConsoleLoggerConfiguration consoleLoggerConfiguration = new ConsoleLoggerConfiguration()
		{
#if DEBUG
			Severity = Severity.Debug
#else
			Severity = Severity.Info
#endif
		};

		public static void SetConsoleLoggerWriteAction(Action<string> writeAction)
		{
			consoleLoggerConfiguration.WriteAction = (s) => {
				try {
					writeAction(s);
				} catch (Exception) { }
			};
		}

		public static Logger Create(Type type)
		{
			return new Logger(type, () => fileLoggerConfiguration, () => consoleLoggerConfiguration);
		}
	}
}
