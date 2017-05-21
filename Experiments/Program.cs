using System.IO;
using Game;
using Game.State;
using Game.Strategy;
using Game.Types;

namespace Experiments
{
	class Program
	{
		private static void Main(string[] args)
		{
			var state = @"
SAMPLES 0 178 0 0 0 0 0 3 2 2 4 3
DIAGNOSIS 0 210 0 0 0 1 0 4 0 3 4 4
5 5 5 4 5
4
32 0 3 0 -1 -1 -1 -1 -1 -1
29 1 2 E 10 3 0 3 0 2
30 1 3 A 30 0 3 3 5 3
31 1 3 0 -1 -1 -1 -1 -1 -1

".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 332 };
			gameState.projects.Add(new Project(3, 0, 0, 3, 3));
			gameState.projects.Add(new Project(0, 0, 3, 3, 3));
			gameState.projects.Add(new Project(0, 3, 3, 3, 0));
			var robotStrategy = new AcquireStrategy(gameState);
			gameState.robotStrategy = robotStrategy;
			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
