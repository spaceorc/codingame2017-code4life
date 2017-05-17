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
LABORATORY 0 127 0 0 0 0 0 2 4 3 2 2
SAMPLES 0 40 0 2 2 3 1 0 0 0 1 1
5 3 3 2 4
10
23 1 3 D 30 3 5 3 0 3
24 1 3 C 40 3 6 3 0 0
6 -1 3 C 40 0 7 0 0 0
10 -1 3 A 30 0 3 3 5 3
16 -1 2 C 30 0 0 6 0 0
14 -1 3 B 50 7 3 0 0 0
18 -1 3 A 50 3 0 0 0 7
19 -1 3 B 40 7 0 0 0 0
20 -1 3 A 40 0 0 0 0 7
22 -1 3 E 40 0 0 0 7 0
".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 292 };
			gameState.projects.Add(new Project(3, 3, 3, 0, 0));
			gameState.projects.Add(new Project(3, 3, 0, 0, 3));
			gameState.projects.Add(new Project(0, 0, 4, 4, 0));
			var robotStrategy = new ProduceStrategy(gameState);
			gameState.robotStrategy = robotStrategy;

			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
