using Game.State;

namespace Game.Strategy
{
	public class Strategy
	{
		public IRobotStrategy robotStrategy;
		private readonly GameState gameState;
		
		public Strategy(GameState gameState)
		{
			this.gameState = gameState;
			robotStrategy = new InitialStrategy(gameState);
		}

		public void Iteration(TurnState turnState)
		{
			while (true)
			{
				var newState = robotStrategy.Process(turnState);
				if (newState == null)
					return;
				robotStrategy = newState;
			}
		}

		public void Dump(string gameStateRef)
		{
			robotStrategy.Dump(gameStateRef);
		}
	}
}