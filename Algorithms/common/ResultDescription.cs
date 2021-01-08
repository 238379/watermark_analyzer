namespace Algorithms.common
{
	public class ResultDescription
	{
		public readonly string Description;

		public ResultDescription(string description)
		{
			Description = description;
		}

		public override string ToString() => Description;
	}
}
