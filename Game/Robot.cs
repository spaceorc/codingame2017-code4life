namespace Game
{
	public class Robot
	{
		public readonly int eta;
		public readonly int[] expertise;
		public readonly int score;
		public readonly int[] storage;
		public readonly string target;

		public Robot(string target, int eta, int score, int storageA, int storageB, int storageC, int storageD, int storageE, int expertiseA, int expertiseB, int expertiseC, int expertiseD, int expertiseE)
		{
			this.target = target;
			this.eta = eta;
			this.score = score;
			storage = new[] {storageA, storageB, storageC, storageD, storageE};
			expertise = new[] {expertiseA, expertiseB, expertiseC, expertiseD, expertiseE};
		}
	}
}