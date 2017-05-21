using System;
using System.Collections.Generic;
using System.Linq;
using Game.Types;

namespace Game.Strategy
{
	public class GatherOrderComparerBuilder
	{
		private readonly GatherOrderComparerBuilder parent;
		private readonly IComparer<GatherOrder> comparer;
		private const int EQUAL = 0;
		private const int X_BETTER = -1;
		private const int Y_BETTER = 1;

		private GatherOrderComparerBuilder(GatherOrderComparerBuilder parent, IComparer<GatherOrder> comparer)
		{
			this.parent = parent;
			this.comparer = comparer;
		}

		public static GatherOrderComparerBuilder Start()
		{
			return new GatherOrderComparerBuilder(null, NullComparer.Default);
		}

		public GatherOrderComparerBuilder Health()
		{
			return new GatherOrderComparerBuilder(this, HealthComparer.Default);
		}

		public GatherOrderComparerBuilder UsedMolecules()
		{
			return new GatherOrderComparerBuilder(this, UsedMoleculesComparer.Default);
		}

		public GatherOrderComparerBuilder Expertise(Robot robot)
		{
			return new GatherOrderComparerBuilder(this, new ExpertiseComparer(robot));
		}

		public GatherOrderComparerBuilder Custom(Func<GatherOrder, GatherOrder, int> customCompare)
		{
			return new GatherOrderComparerBuilder(this, new CustomComparer(customCompare));
		}

		public IComparer<GatherOrder> Build()
		{
			var comparers = new List<IComparer<GatherOrder>>();
			for (var builder = this; builder != null; builder = builder.parent)
				comparers.Add(builder.comparer);
			comparers.Reverse();
			return new CompositeComparer(comparers);
		}

		private class CompositeComparer : IComparer<GatherOrder>
		{
			private readonly List<IComparer<GatherOrder>> comparers;

			public CompositeComparer(List<IComparer<GatherOrder>> comparers)
			{
				this.comparers = comparers;
			}

			public int Compare(GatherOrder x, GatherOrder y)
			{
				foreach (var comparer in comparers)
				{
					var result = comparer.Compare(x, y);
					if (result != 0)
						return result;
				}
				return EQUAL;
			}
		}

		private class CustomComparer : IComparer<GatherOrder>
		{
			private readonly Func<GatherOrder, GatherOrder, int> customCompare;

			public CustomComparer(Func<GatherOrder, GatherOrder, int> customCompare)
			{
				this.customCompare = customCompare;
			}

			public int Compare(GatherOrder x, GatherOrder y)
			{
				return customCompare(x, y);
			}
		}

		private class NullComparer : IComparer<GatherOrder>
		{
			public static readonly IComparer<GatherOrder> Default = new NullComparer();

			public int Compare(GatherOrder x, GatherOrder y)
			{
				if (x == null && y == null)
					return EQUAL;
				if (x == null)
					return Y_BETTER;
				if (y == null)
					return X_BETTER;
				return EQUAL;
			}
		}

		private class HealthComparer : IComparer<GatherOrder>
		{
			public static readonly IComparer<GatherOrder> Default = new HealthComparer();

			public int Compare(GatherOrder x, GatherOrder y)
			{
				if (x.health > y.health)
					return X_BETTER;
				if (x.health < y.health)
					return Y_BETTER;
				return EQUAL;
			}
		}

		private class UsedMoleculesComparer : IComparer<GatherOrder>
		{
			public static readonly IComparer<GatherOrder> Default = new UsedMoleculesComparer();

			public int Compare(GatherOrder x, GatherOrder y)
			{
				if (x.usedMolecules.totalCount < y.usedMolecules.totalCount)
					return X_BETTER;
				if (x.usedMolecules.totalCount > y.usedMolecules.totalCount)
					return Y_BETTER;
				return EQUAL;
			}
		}

		private class ExpertiseComparer : IComparer<GatherOrder>
		{
			private readonly Robot robot;

			public ExpertiseComparer(Robot robot)
			{
				this.robot = robot;
			}

			public int Compare(GatherOrder x, GatherOrder y)
			{
				var xexpertise = robot.expertise.Add(x.additionalExpertise);
				var xminExpertise = xexpertise.counts.Min();
				var yexpertise = robot.expertise.Add(y.additionalExpertise);
				var yminExpertise = yexpertise.counts.Min();
				if (xminExpertise > yminExpertise)
					return X_BETTER;
				if (xminExpertise < yminExpertise)
					return Y_BETTER;
				var xminExpertiseCount = xexpertise.counts.Count(t => t == xminExpertise);
				var yminExpertiseCount = yexpertise.counts.Count(t => t == yminExpertise);
				if (xminExpertiseCount > yminExpertiseCount)
					return Y_BETTER;
				if (xminExpertiseCount < yminExpertiseCount)
					return X_BETTER;
				return EQUAL;
			}
		}
	}
}