using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class CollectStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public CollectStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			if (turnState.carriedSamples.Count == Constants.MAX_TRAY)
				return new AnalyzeStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.SAMPLES) == GoToResult.Arrived)
				turnState.robot.Connect(SelectNewSampleRank(turnState));
			return null;
		}

		private static int SelectNewSampleRank(TurnState turnState)
		{
			var robot = turnState.robots[0];
			var minExpertise = robot.expertise.OrderBy(x => x).Skip(Settings.RANK_MIN_SKIP).First();
			if (minExpertise < Settings.RANK_2_LIMIT)
				return 1;
			if (minExpertise < Settings.RANK_3_LIMIT)
				return 2;
			return 3;

			/*
			var expertise = robot.expertise.Sum();
			var cost = turnState.samples.Where(x => x.carriedBy == 0).Select(x => x.cost.Sum()).Sum();
			var available = turnState.available.Sum();
			if (available + expertise - cost > Settings.RANK_3_LIMIT)
				return 3;
			if (available + expertise - cost > Settings.RANK_2_LIMIT)
				return 2;
			return 1;
			*/
		}
	}
}