namespace Game
{
	public class Project
	{
		private readonly int[] expertise;

		public Project(int a, int b, int c, int d, int e)
		{
			expertise = new[] {a, b, c, d, e};
		}

		public string Dump()
		{
			return $"new {nameof(Project)}({expertise[0]},{expertise[1]},{expertise[2]},{expertise[3]},{expertise[4]})";
		}
	}
}