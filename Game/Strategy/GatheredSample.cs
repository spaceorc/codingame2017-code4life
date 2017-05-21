using Game.Types;

namespace Game.Strategy
{
	public class GatheredSample
	{
		public readonly Sample sample;
		public readonly int health;
		public readonly GatheredSampleType type;
		public readonly MoleculeSet moleculesToGather;

		public GatheredSample(Sample sample, int health, GatheredSampleType type, MoleculeSet moleculesToGather)
		{
			this.sample = sample;
			this.health = health;
			this.type = type;
			this.moleculesToGather = moleculesToGather;
		}
	}
}