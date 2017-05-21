using Game.State;

namespace Game.Strategy
{
	public class InitialStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public InitialStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			return new AcquireStrategy(gameState);
		}
	}
}