using System.Collections.Generic;
using System.Linq;
using Game.Types;

namespace Game.Strategy
{
	public class ProduceOrderDefaultComparer : IComparer<ProduceOrder>
	{
		public readonly Robot robot;

		public ProduceOrderDefaultComparer(Robot robot)
		{
			this.robot = robot;
		}

		private const int EQUAL = 0;
		private const int X_BETTER = -1;
		private const int Y_BETTER = 1;

		public int Compare(ProduceOrder x, ProduceOrder y)
		{
			if (x == null && y == null)
				return EQUAL;
			if (x == null)
				return Y_BETTER;
			if (y == null)
				return X_BETTER;
			if (x.health > y.health)
				return X_BETTER;
			if (x.health < y.health)
				return Y_BETTER;
			if (x.usedMolecules.totalCount < y.usedMolecules.totalCount)
				return X_BETTER;
			if (x.usedMolecules.totalCount > y.usedMolecules.totalCount)
				return Y_BETTER;

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