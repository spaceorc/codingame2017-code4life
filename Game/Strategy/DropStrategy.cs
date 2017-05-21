using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	//public class DropStrategy : RobotStrategyBase
	//{
	//	private readonly GameState gameState;

	//	public DropStrategy(GameState gameState)
	//	{
	//		this.gameState = gameState;
	//	}

	//	public override IRobotStrategy Process(TurnState turnState)
	//	{
	//		if (turnState.robot.samples.Count < Constants.MAX_TRAY)
	//			return new CollectStrategy(gameState);
	//		if (turnState.robot.samples.Any(x => turnState.robot.CanGather(turnState, x)))
	//			return new GatherStrategy(gameState);
	//		if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
	//		{
	//			var sampleToDrop = turnState.robot.samples.OrderByDescending(x => x.health).First();
	//			turnState.robot.Connect(sampleToDrop.sampleId);
	//		}
	//		return null;
	//		/*if (turnState.robot.samples.Any(x => turnState.robot.CanGather(turnState, x)))
	//			return new GatherStrategy(gameState);
	//		if (!turnState.robot.samples.Any())
	//			return new CollectStrategy(gameState);
	//		if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
	//		{
	//			var sampleToDrop = turnState.robot.samples.OrderByDescending(x => x.health).First();
	//			turnState.robot.Connect(sampleToDrop.sampleId);
	//		}
	//		return null;*/
	//	}
	//}
}