namespace Game
{
	public class Sample
	{
		public readonly int sampleId;
		public readonly int carriedBy;
		public readonly int rank;
		public readonly string expertiseGain;
		public readonly int health;
		public readonly int[] cost;

		public Sample(int sampleId, int carriedBy, int rank, string expertiseGain, int health, int costA, int costB, int costC, int costD, int costE)
		{
			this.sampleId = sampleId;
			this.carriedBy = carriedBy;
			this.rank = rank;
			this.expertiseGain = expertiseGain;
			this.health = health;
			cost = new [] { costA, costB, costC, costD, costE };
		}
	}
}