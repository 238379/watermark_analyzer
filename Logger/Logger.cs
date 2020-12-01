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

		public void LogDebug(object log)
		{
			Log(log, Severity.Debug);
		}

		public void LogInfo(object log)
		{
			Log(log, Severity.Info);
		}

		public void LogError(object log)
		{
			Log(log, Severity.Error);
		}

		private void Log(object log, Severity severity)
		{
			fileOutput.Log(log.ToString(), severity, type);
			consoleOutput.Log(log.ToString(), severity, type);
		}
	}
}
