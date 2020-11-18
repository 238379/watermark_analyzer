using LoggerUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;

namespace Generators
{
	public class InternetImageGenerator : Generator
	{
		private readonly Logger logger;

		public InternetImageGenerator(Dictionary<string, dynamic> generatorParameters) : base(generatorParameters)
		{
			logger = LoggerFactory.Create(GetType());
		}

		public override Bitmap Generate()
		{
			try
			{
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://loremflickr.com/800/800");
				webRequest.Timeout = 5000;

				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					logger.LogDebug($"Redirected to {webResponse.ResponseUri}.");
					System.IO.Stream responseStream = webResponse.GetResponseStream();
					return new Bitmap(responseStream);
				}
			}
			catch(Exception e)
			{
				logger.LogError(e.Message);
				logger.LogDebug(e.StackTrace);
			}
			return null;
		}
	}
}
