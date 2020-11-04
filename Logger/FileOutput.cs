using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoggerUtils
{
	class FileOutput : Output
	{
		private static readonly object locker = new object();

		private readonly Func<FileLoggerConfiguration> loggerConfigurationProvider;

		public FileOutput(Func<FileLoggerConfiguration> loggerConfigurationProvider) : base(loggerConfigurationProvider)
		{
			this.loggerConfigurationProvider = loggerConfigurationProvider;
		}

		protected override void Log(string log)
		{
			var config = loggerConfigurationProvider();
			lock (locker)
			{
				if (!File.Exists(config.filePath))
				{
					var dirPath = Path.GetDirectoryName(config.filePath);
					if (!Directory.Exists(dirPath))
					{
						Directory.CreateDirectory(dirPath);
					}
					File.Create(config.filePath).Dispose();
				}
				File.AppendAllText(config.filePath, log + Environment.NewLine);
			}
		}
	}
}
