using System;
using System.IO;

namespace LoggerUtils
{
	public class FileLoggerConfiguration : LoggerConfiguration
	{
		public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/analyzer.log");
	}
}
