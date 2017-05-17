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
			if (!turnState.robot.samples.All(x => x.Diagnosed))
			{
				if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
					turnState.robot.Connect(turnState.robot.samples.First(x => !x.Diagnosed).sampleId);
				return null;
			}
			if (turnState.robot.samples.Count < Constants.MAX_TRAY)
			{
				var samples = turnState.robot.samples.ToList();
				while (samples.Count < Constants.MAX_TRAY)
				{
					var rank = CollectStrategy.SelectNewSampleRank(turnState.robot, samples);
					samples.Add(new Sample(-1, 0, rank, "0", 0, -1, -1, -1, -1, -1));
				}
				var ranks = samples.Skip(turnState.robot.samples.Count).Select(x => x.rank).ToList();
				foreach (var rank in ranks)
				{
					var sample = turnState.cloudSamples.FirstOrDefault(x => x.rank == rank && turnState.robot.CanGather(turnState, x));
					if (sample != null)
					{
						if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
							turnState.robot.Connect(sample.sampleId);
						return null;
					}
				}
			}
			var sampleToDrop = turnState.robot.samples.FirstOrDefault(x => !turnState.robot.CanGather(turnState, x));
			if (sampleToDrop != null)
			{
				if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
					turnState.robot.Connect(sampleToDrop.sampleId);
				return null;
			}
			return new GatherStrategy(gameState);
		}
	}
}