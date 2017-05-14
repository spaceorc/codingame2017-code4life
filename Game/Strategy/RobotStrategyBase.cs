using System;
using Game.State;

namespace Game.Strategy
{
	public abstract class RobotStrategyBase : IRobotStrategy
	{
		public abstract IRobotStrategy Process(TurnState turnState);

		public void Dump(string gameStateRef)
		{
			Console.Error.WriteLine($"var robotStrategy = new {GetType().Name}({gameStateRef});");
			Console.Error.WriteLine($"{gameStateRef}.{nameof(GameState.strategy)}.{nameof(Strategy.robotStrategy)} = robotStrategy;");
			DumpState($"robotStrategy");
		}

		protected virtual void DumpState(string robotStrategyRef)
		{
		}
	}
}