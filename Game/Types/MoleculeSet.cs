using System.Linq;

namespace Game.Types
{
	public class MoleculeSet
	{
		public readonly int[] counts;

		public MoleculeSet()
			: this(new int[Constants.MOLECULE_TYPE_COUNT])
		{
		}

		public MoleculeSet(int[] counts)
		{
			this.counts = counts;
		}

		public MoleculeSet(int a, int b, int c, int d, int e)
			: this(new[] {a, b, c, d, e})
		{
		}

		public int TotalCount => counts.Sum();

		public override string ToString()
		{
			return counts.ToMoleculesString();
		}

		public MoleculeSet Gain(MoleculeType gain)
		{
			var res = counts.ToArray();
			res[(int)gain]++;
			return new MoleculeSet(res);
		}

		public MoleculeSet Add(MoleculeSet other)
		{
			if (other == null)
				return this;
			var res = counts.ToArray();
			for (var i = 0; i < other.counts.Length; i++)
				res[i] += other.counts[i];
			return new MoleculeSet(res);
		}

		public bool IsSupersetOf(MoleculeSet other) => other.IsSubsetOf(this);

		public bool IsSubsetOf(MoleculeSet other)
		{
			for (var i = 0; i < counts.Length; i++)
			{
				if (counts[i] > other.counts[i])
					return false;
			}
			return true;
		}

		public MoleculeSet Subtract(MoleculeSet other)
		{
			if (other == null)
				return this;
			var res = counts.ToArray();
			for (var i = 0; i < counts.Length; i++)
			{
				res[i] -= other.counts[i];
				if (res[i] < 0)
					res[i] = 0;
			}
			return new MoleculeSet(res);
		}
	}
}