﻿namespace Game.Types
{
	public class Sample
	{
		public readonly int sampleId;
		public readonly int carriedBy;
		public readonly int rank;
		public readonly string gainString;
		public readonly MoleculeType gain;
		public readonly int health;
		public readonly MoleculeSet cost;

		public Sample(int sampleId, int carriedBy, int rank, string gainString, int health, int costA, int costB, int costC, int costD, int costE)
		{
			this.sampleId = sampleId;
			this.carriedBy = carriedBy;
			this.rank = rank;
			this.gainString = gainString;
			gain = (MoleculeType)(this.gainString[0] - 'A');
			this.health = health;
			cost = new MoleculeSet(costA, costB, costC, costD, costE);
		}

		public bool Diagnosed => cost.counts[0] >= 0;

		public override string ToString()
		{
			return $"{sampleId} [owner:{carriedBy}] rank:{rank}, health:{health}, gain:{gain}, {cost}";
		}
	}
}