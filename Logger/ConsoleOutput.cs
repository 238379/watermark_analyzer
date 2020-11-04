﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerUtils
{
	class ConsoleOutput : Output
	{
		private readonly Func<ConsoleLoggerConfiguration> loggerConfigurationProvider;

		public ConsoleOutput(Func<ConsoleLoggerConfiguration> loggerConfigurationProvider) : base(loggerConfigurationProvider)
		{
			this.loggerConfigurationProvider = loggerConfigurationProvider;
		}

		protected override void Log(string log)
		{
			loggerConfigurationProvider().WriteAction(log + Environment.NewLine);
		}
	}
}
