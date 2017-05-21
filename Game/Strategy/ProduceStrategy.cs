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
			var produceOrder = ProduceOrder.GetProduceOrders(gameState, turnState.robot).SelectBestOrder(new ProduceOrderDefaultComparer(turnState.robot));
			var canProduceSample = produceOrder?.producedSamples.FirstOrDefault();
			if (canProduceSample == null)
				return new GatherStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.LABORATORY) == GoToResult.Arrived)
				turnState.robot.Connect(canProduceSample.sample.sampleId);
			return null;
		}
	}
}