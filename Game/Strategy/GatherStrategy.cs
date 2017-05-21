using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class GatherStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public GatherStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			var enemyGatherOrder = turnState.enemy.target == ModuleType.MOLECULES 
				|| turnState.enemy.target == ModuleType.LABORATORY
				|| turnState.enemy.target == ModuleType.DIAGNOSIS && turnState.enemy.samples.All(x => x.Diagnosed)
				? GatherOrder.GetGatherOrders(gameState, turnState, turnState.enemy).SelectBestOrder(GatherOrderDefaultComparer.Build(turnState.enemy))
				: null;

			var gatherOrder = GatherOrder.GetGatherOrders(gameState, turnState, turnState.robot).SelectBestOrder(GatherOrderDefaultComparer.Build(turnState.robot));
			if (gatherOrder == null || !gatherOrder.gatheredSamples.Any())
			{
				if (turnState.robot.At(ModuleType.MOLECULES) && turnState.robot.storage.totalCount < Constants.MAX_STORAGE)
				{
					// try to prevent enemy or gather "after recycle" samples
					if (enemyGatherOrder != null)
					{
						foreach (var enemyGatheredSample in enemyGatherOrder.gatheredSamples.Concat(enemyGatherOrder.gatheredAfterRecycleSamples))
						{
							var enemyMoleculesToGather = enemyGatheredSample.moleculesToGather.Intersect(turnState.available);
							if (enemyMoleculesToGather.totalCount > 0)
							{
								turnState.robot.Connect(enemyMoleculesToGather.Max()); // todo prevent with best selection (maybe take rare)
								return null;
							}
						}
					}
					if (gatherOrder != null)
					{
						foreach (var gatheredSample in gatherOrder.gatheredAfterRecycleSamples)
						{
							var moleculesToGather = gatheredSample.moleculesToGather.Intersect(turnState.available);
							if (moleculesToGather.totalCount > 0)
							{
								turnState.robot.Connect(moleculesToGather.Max()); // todo get with best selection (maybe take rare)
								return null;
							}
						}
						
					}
				}
				if (gatherOrder != null && gatherOrder.producedSamples.Any())
					return new ProduceStrategy(gameState);
				return new DropStrategy(gameState);
			}

			if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.OnTheWay)
				return null;

			foreach (var gatheredSample in gatherOrder.gatheredSamples)
			{
				var moleculesToGather = gatheredSample.moleculesToGather.Intersect(turnState.available);
				if (moleculesToGather.totalCount > 0)
				{
					if (enemyGatherOrder != null)
					{
						foreach (var enemyGatheredSample in enemyGatherOrder.gatheredSamples.Concat(enemyGatherOrder.gatheredAfterRecycleSamples))
						{
							var enemyMoleculesToGather = enemyGatheredSample.moleculesToGather.Intersect(turnState.available);
							var moleculesWeAllNeed = enemyMoleculesToGather.Intersect(moleculesToGather);
							if (moleculesWeAllNeed.totalCount > 0)
							{
								turnState.robot.Connect(moleculesWeAllNeed.Max()); // todo prevent with best selection (maybe take rare)
								return null;
							}
						}
					}
					turnState.robot.Connect(moleculesToGather.Max()); // todo get with best selection (maybe take rare)
					return null;
				}
			}
			turnState.robot.Wait();
			return null;
		}
	}
}