using System;
using System.Linq;

namespace Game
{
	public class Strategy
	{
		private readonly GameState gameState;
		public RobotState robotState;

		public enum RobotState
		{
			Collect,
			Analyze,
			Gather,
			Drop,
			Produce
		}

		public Strategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public void Iteration(TurnState turnState)
		{
			var carriedSamples = turnState.samples.Where(x => x.carriedBy == 0).ToList();
			var cloudSamples = turnState.samples.Where(x => x.carriedBy == -1).ToList();
			var otherSamples = turnState.samples.Where(x => x.carriedBy == 1).ToList();
			while (true)
			{
				switch (robotState)
				{
					case RobotState.Collect:
						if (carriedSamples.Count == Constants.MAX_TRAY)
						{
							robotState = RobotState.Analyze;
							continue;
						}
						if (turnState.robots[0].target != "SAMPLES" || turnState.robots[0].eta != 0)
						{
							Console.WriteLine("GOTO SAMPLES");
							return;
						}
						else
						{
							Console.WriteLine($"CONNECT {GetAvailableRank(turnState)}");
							return;
						}
					case RobotState.Analyze:
						if (carriedSamples.All(x => x.Diagnosed))
						{
							robotState = RobotState.Gather;
							continue;
						}
						if (turnState.robots[0].target != "DIAGNOSIS" || turnState.robots[0].eta != 0)
						{
							Console.WriteLine("GOTO DIAGNOSIS");
							return;
						}
						else
						{
							Console.WriteLine($"CONNECT {carriedSamples.First(x => !x.Diagnosed).sampleId}");
							return;
						}
					case RobotState.Gather:
						var canGatherSample = carriedSamples.OrderByDescending(x => x.health).FirstOrDefault(x => CanGather(turnState, x));
						if (canGatherSample == null)
						{
							robotState = RobotState.Drop;
							continue;
						}
						if (CanProduce(turnState, canGatherSample))
						{
							robotState = RobotState.Produce;
							continue;
						}
						if (turnState.robots[0].target != "MOLECULES" || turnState.robots[0].eta != 0)
						{
							Console.WriteLine("GOTO MOLECULES");
							return;
						}
						else
						{
							Console.WriteLine($"CONNECT {GetUngatheredType(turnState, canGatherSample).ToMoleculeType()}");
							return;
						}
					case RobotState.Drop:
						if (carriedSamples.Any(x => CanGather(turnState, x)))
						{
							robotState = RobotState.Gather;
							continue;
						}
						if (!carriedSamples.Any())
						{
							robotState = RobotState.Collect;
							continue;
						}
						if (turnState.robots[0].target != "DIAGNOSIS" || turnState.robots[0].eta != 0)
						{
							Console.WriteLine("GOTO DIAGNOSIS");
							return;
						}
						else
						{
							var sampleToDrop = carriedSamples.OrderByDescending(x => x.health).First();
							Console.WriteLine($"CONNECT {sampleToDrop.sampleId}");
							return;
						}
					case RobotState.Produce:
						var canProduceSample = carriedSamples.OrderByDescending(x => x.health).FirstOrDefault(x => CanProduce(turnState, x));
						if (canProduceSample == null)
						{
							robotState = RobotState.Gather;
							continue;
						}
						if (turnState.robots[0].target != "LABORATORY" || turnState.robots[0].eta != 0)
						{
							Console.WriteLine("GOTO LABORATORY");
							return;
						}
						else
						{
							Console.WriteLine($"CONNECT {canProduceSample.sampleId}");
							return;
						}
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private bool CanGather(TurnState turnState, Sample sample)
		{
			var robot = turnState.robots[0];
			int extra = 0;
			for (int i = 0; i < sample.cost.Length; i++)
			{
				if (robot.storage[i] + robot.expertise[i] + turnState.available[i] < sample.cost[i])
					return false;
				var extraOfType = sample.cost[i] - (robot.storage[i] + robot.expertise[i]);
				if (extraOfType > 0)
					extra += extraOfType;
			}
			if (robot.storage.Sum() + extra > Constants.MAX_STORAGE)
				return false;
			return true;
		}

		private int GetUngatheredType(TurnState turnState, Sample sample)
		{
			var robot = turnState.robots[0];
			var minAvailable = int.MaxValue;
			var result = -1;
			for (int i = 0; i < sample.cost.Length; i++)
			{
				if (robot.storage[i] + robot.expertise[i] < sample.cost[i])
				{
					if (turnState.available[i] < minAvailable)
					{
						result = i;
						minAvailable = turnState.available[i];
					}
				}
			}
			return result;
		}

		private bool CanProduce(TurnState turnState, Sample sample)
		{
			var robot = turnState.robots[0];
			for (int i = 0; i < sample.cost.Length; i++)
			{
				if (robot.storage[i] + robot.expertise[i] < sample.cost[i])
					return false;
			}
			return true;
		}

		private int GetAvailableRank(TurnState turnState)
		{
			var robot = turnState.robots[0];
			var expertise = robot.expertise.Sum();
			var cost = turnState.samples.Where(x => x.carriedBy == 0).Select(x => x.cost.Sum()).Sum();
			var available = turnState.available.Sum();
			if (available + expertise - cost > Settings.RANK_3_LIMIT)
				return 3;
			if (available + expertise - cost > Settings.RANK_2_LIMIT)
				return 2;
			return 1;
		}

		public void Dump(string gameStateRef)
		{
			Console.Error.WriteLine($"{gameStateRef}.{nameof(gameState.strategy)}.{nameof(robotState)} = {nameof(RobotState)}.{robotState};");
		}
	}
}