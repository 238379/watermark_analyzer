using System;
using System.Collections.Generic;
using System.Drawing;

namespace Generators
{
	public abstract class Generator
	{
		protected readonly Dictionary<string, dynamic> generatorParameters;

		public static Generator Create(string generatorName, Dictionary<string, dynamic> parameters)
		{
			return generatorName switch
			{
				InternetImageGenerator.GENERATOR_NAME => new InternetImageGenerator(parameters),
				TextImageGenerator.GENERATOR_NAME => new TextImageGenerator(parameters),
				_ => throw new ArgumentException($"Unknown generatorName '{generatorName}'."),
			};
		}

		public Generator(Dictionary<string, dynamic> generatorParameters)
		{
			this.generatorParameters = generatorParameters;
		}

		public abstract Bitmap Generate();
	}
}
