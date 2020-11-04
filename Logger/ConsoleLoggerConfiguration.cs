using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerUtils
{
	public class ConsoleLoggerConfiguration : FileLoggerConfiguration
	{
		public Action<string> WriteAction = _ => { };
	}
}
