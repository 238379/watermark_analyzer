using Common;
using LoggerUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;

namespace Generators
{
	public class InternetImageGenerator : Generator
	{
		public const string GENERATOR_NAME = "INTERNET_IMAGE_GENERATOR";

		private readonly Logger logger;

		private readonly PullingQueue<Bitmap> pullingQueue;

		public InternetImageGenerator(Dictionary<string, dynamic> generatorParameters) : base(generatorParameters)
		{
			logger = LoggerFactory.Create(GetType());
			pullingQueue = new PullingQueue<Bitmap>(() => Get(), LogError, 5, 3000);
		}

		public override Bitmap Generate()
		{
			return pullingQueue.Pull();
		}

		private async Task<Bitmap> Get()
		{
			var sw = Stopwatch.StartNew();

			await InternetTools.WaitForInternetConnection(TimeSpan.FromSeconds(1));

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://loremflickr.com/800/800");
			webRequest.Timeout = 5000;

			using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				logger.LogDebug($"Redirected to {webResponse.ResponseUri}.");
				System.IO.Stream responseStream = webResponse.GetResponseStream();
				logger.LogDebug($"Got image in: {sw.ElapsedMilliseconds}ms.");
				return new Bitmap(responseStream);
			}
		}

		private void LogError(Exception e)
		{
			logger.LogError(e.Message);
			logger.LogDebug(e.StackTrace);
		}
	}
}
