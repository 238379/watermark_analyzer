using System;
using System.Diagnostics;

namespace LoggerUtils
{
	abstract class Output
	{
		private Func<LoggerConfiguration> loggerConfigurationProvider;

		public Output(Func<LoggerConfiguration> loggerConfigurationProvider)
		{
			this.loggerConfigurationProvider = loggerConfigurationProvider;
		}

		public void Log(string log, Severity severity, Type type)
		{
			if(SeverityMatches(severity))
			{
				Log(ConcatLog(log, severity, type));
			}
		}

		protected abstract void Log(string log);

		protected string ConcatLog(string log, Severity severity, Type type)
		{
			return $"{Process.GetCurrentProcess().Id} {DateTime.Now} [{severity}] {type.FullName}: '{log}'";
		}

		private bool SeverityMatches(Severity severity)
		{
			return loggerConfigurationProvider().Severity <= severity;
		}
	}
}
