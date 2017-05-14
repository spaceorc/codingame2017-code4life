﻿using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class DropStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public DropStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			if (turnState.carriedSamples.Any(x => turnState.robot.CanGather(turnState, x)))
				return new GatherStrategy(gameState);
			if (!turnState.carriedSamples.Any())
				return new CollectStrategy(gameState);
			if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
			{
				var sampleToDrop = turnState.carriedSamples.OrderByDescending(x => x.health).First();
				turnState.robot.Connect(sampleToDrop.sampleId);
			}
			return null;
		}
	}
}