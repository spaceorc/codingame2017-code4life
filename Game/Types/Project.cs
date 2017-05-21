namespace Game.Types
{
	public class Project
	{
		public readonly MoleculeSet expertise;

		public Project(int a, int b, int c, int d, int e)
		{
			expertise = new MoleculeSet(a, b, c, d, e);
		}

		public bool IsComplete(Robot robot, MoleculeSet additionalExpertise = null)
		{
			var robotExp = robot.expertise.Add(additionalExpertise);
			return expertise.Subtract(robotExp).totalCount == 0;
		}

		public string Dump()
		{
			return $"new {nameof(Project)}({expertise.counts[0]},{expertise.counts[1]},{expertise.counts[2]},{expertise.counts[3]},{expertise.counts[4]})";
		}
	}
}