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
			var min = robot.expertise.OrderBy(x => x).First();
			var min2 = robot.expertise.OrderBy(x => x).Skip(Settings.RANK_MIN_SKIP).First();
			if (min2 < Settings.RANK_2_LIMIT)
				return 1;
			if (min2 < Settings.RANK_3_LIMIT)
				return 2;
			if (min < Settings.RANK_3_LIMIT)
			{
				if (!turnState.carriedSamples.Any(x => x.rank < 3))
					return 2;
			}
			return 3;
		}
	}
}