using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class GatherStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public GatherStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			var canGatherSample = turnState.carriedSamples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x));
			if (canGatherSample == null)
				return new DropStrategy(gameState);
			if (turnState.robot.CanProduce(canGatherSample))
				return new ProduceStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.Arrived)
				turnState.robot.Connect(GetUngatheredType(turnState, canGatherSample));
			return null;
		}

		private static MoleculeType GetUngatheredType(TurnState turnState, Sample sample, int[] additionalExpertise = null)
		{
			var robot = turnState.robots[0];
			var minAvailable = int.MaxValue;
			var result = MoleculeType.Unknown;
			for (var i = 0; i < sample.cost.Length; i++)
			{
				if (robot.storage[i] + robot.expertise[i] + (additionalExpertise?[i] ?? 0) < sample.cost[i])
				{
					if (turnState.available[i] < minAvailable)
					{
						result = (MoleculeType)i;
						minAvailable = turnState.available[i];
					}
				}
			}
			return result;
		}
	}
}