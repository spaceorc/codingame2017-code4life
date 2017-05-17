using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	// pack: 0
	public class CollectStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public CollectStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			if (turnState.robot.samples.Count == Constants.MAX_TRAY)
				return new AnalyzeStrategy(gameState);

			var samples = turnState.robot.samples.ToList();
			while (samples.Count < Constants.MAX_TRAY)
			{
				var rank = SelectNewSampleRank(turnState.robot, samples);
				samples.Add(new Sample(-1, 0, rank, "0", 0, -1, -1, -1, -1, -1));
			}
			var ranks = samples.Skip(turnState.robot.samples.Count).Select(x => x.rank).ToList();
			var cloudSamples = turnState.cloudSamples.ToList();
			foreach (var rank in ranks)
			{
				var sample = cloudSamples.FirstOrDefault(x => x.rank == rank && turnState.robot.CanGather(turnState, x));
				if (sample != null)
					cloudSamples.Remove(sample);
				else
				{
					if (turnState.robot.GoTo(ModuleType.SAMPLES) == GoToResult.Arrived)
						turnState.robot.Connect(rank);
					return null;
				}
			}
			return new AnalyzeStrategy(gameState);
		}

		public static int SelectNewSampleRank(Robot robot, List<Sample> carriedSamples)
		{
			var sum = robot.expertise.totalCount;
			if (sum < 5)
				return 1;
			if (sum < 7)
			{
				if (carriedSamples.Count(x => x.rank == 1) < 2)
					return 1;
				return 2;
			}
			if (sum < 9)
			{
				if (carriedSamples.Count(x => x.rank == 1) < 1)
					return 1;
				return 2;
			}
			if (sum < 11)
			{
				if (carriedSamples.Count(x => x.rank == 2) < 2)
					return 2;
				return 3;
			}
			if (sum < 13)
			{
				if (carriedSamples.Count(x => x.rank == 2) < 1)
					return 2;
				return 3;
			}
			return 3;
		}
	}
}