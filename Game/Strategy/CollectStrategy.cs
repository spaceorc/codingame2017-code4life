using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
			if (turnState.robot.samples.Count == Constants.MAX_TRAY)
				return new AnalyzeStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.SAMPLES) == GoToResult.Arrived)
				turnState.robot.Connect(SelectNewSampleRank(turnState));
			return null;
		}

		private static int SelectNewSampleRank(TurnState turnState)
		{
			var robot = turnState.robots[0];
			var sum = robot.expertise.totalCount;
			if (sum < 3)
				return 1;
			if (sum < 5)
			{
				if (turnState.robot.samples.Count(x => x.rank == 1) < 2)
					return 1;
				return 2;
			}
			if (sum < 7)
			{
				if (turnState.robot.samples.Count(x => x.rank == 1) < 1)
					return 1;
				return 2;
			}
			if (sum < 9)
			{
				if (turnState.robot.samples.Count(x => x.rank == 2) < 2)
					return 2;
				return 3;
			}
			if (sum < 11)
			{
				if (turnState.robot.samples.Count(x => x.rank == 2) < 1)
					return 2;
				return 3;
			}
			return 3;
		}
	}
}