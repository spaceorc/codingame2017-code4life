using System;
using System.Linq;

namespace Game
{
	public class Strategy
	{
		private readonly GameState gameState;

		public Strategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public void Iteration(TurnState turnState)
		{
			var sample = turnState.samples.FirstOrDefault(x => x.carriedBy == 0);
			if (sample == null)
			{
				if (turnState.robots[0].target == "SAMPLES" && turnState.robots[0].eta == 0)
				{
					Console.WriteLine($"CONNECT {1}");
				}
				else
				{
					Console.WriteLine("GOTO SAMPLES");
				}
			}
			else if (sample.cost[0] < 0)
			{
				if (turnState.robots[0].target == "DIAGNOSIS" && turnState.robots[0].eta == 0)
				{
					Console.WriteLine($"CONNECT {sample.sampleId}");
				}
				else
				{
					Console.WriteLine("GOTO DIAGNOSIS");
				}
			}
			else
			{
				var missingType = FindMissingType(turnState.robots[0].storage, sample.cost);
				if (missingType < 0)
				{
					if (turnState.robots[0].target == "LABORATORY" && turnState.robots[0].eta == 0)
					{
						Console.WriteLine($"CONNECT {sample.sampleId}");
					}
					else
					{
						Console.WriteLine("GOTO LABORATORY");
					}
				}
				else
				{
					if (turnState.robots[0].target == "MOLECULES" && turnState.robots[0].eta == 0)
					{
						Console.WriteLine($"CONNECT {(char)('A' + missingType)}");
					}
					else
					{
						Console.WriteLine("GOTO MOLECULES");
					}
				}
			}

		}

		private static int FindMissingType(int[] storage, int[] cost)
		{
			for (var i = 0; i < cost.Length; i++)
			{
				if (cost[i] > storage[i])
					return i;
			}
			return -1;
		}

		public void Dump(string gameStateRef)
		{
		}
	}
}