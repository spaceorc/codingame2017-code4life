using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class ProduceStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public ProduceStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			var canProduceSample = turnState.carriedSamples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanProduce(x));
			if (canProduceSample == null)
				return new GatherStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.LABORATORY) == GoToResult.Arrived)
				turnState.robot.Connect(canProduceSample.sampleId);
			return null;
		}
	}
}