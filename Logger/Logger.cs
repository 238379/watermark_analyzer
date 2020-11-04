using System;
using System.Diagnostics;

namespace LoggerUtils
{
	public class Logger
	{
		private readonly Type type;
		private readonly FileOutput fileOutput;
		private readonly ConsoleOutput consoleOutput;

		public Logger(Type type, Func<FileLoggerConfiguration> fileConfigurationProvider, Func<ConsoleLoggerConfiguration> consoleLoggerConfiguration)
		{
			this.type = type;
			fileOutput = new FileOutput(fileConfigurationProvider);
			consoleOutput = new ConsoleOutput(consoleLoggerConfiguration);
		}

		public void LogDebug(string log)
		{
			Log(log, Severity.Debug);
		}

		public void LogInfo(string log)
		{
			Log(log, Severity.Info);
		}

		public void LogError(string log)
		{
			Log(log, Severity.Error);
		}

		private void Log(string log, Severity severity)
		{
			fileOutput.Log(log, severity, type);
			consoleOutput.Log(log, severity, type);
		}
	}
}
