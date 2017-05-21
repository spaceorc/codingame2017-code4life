using System.Collections.Generic;
using Game.Types;

namespace Game.Strategy
{
	public static class GatherOrderDefaultComparer
	{
		public static IComparer<GatherOrder> Build(Robot robot)
		{
			return GatherOrderComparerBuilder.Start()
				.Health()
				.UsedMolecules()
				.Expertise(robot)
				.Build();
		}
	}
}