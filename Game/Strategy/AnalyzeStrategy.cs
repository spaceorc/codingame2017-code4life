using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class AnalyzeStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public AnalyzeStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			if (turnState.robot.samples.All(x => x.Diagnosed))
				return new GatherStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
				turnState.robot.Connect(turnState.robot.samples.First(x => !x.Diagnosed).sampleId);
			return null;
		}
	}
}