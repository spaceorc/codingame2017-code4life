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
MOLECULES 0 146 0 0 4 0 0 3 1 3 4 1
MOLECULES 0 196 2 0 0 0 0 3 4 3 1 3
3 5 1 5 5
7
30 0 3 B 40 6 3 0 0 3
31 0 3 A 30 0 3 3 5 3
32 0 3 D 50 0 0 7 3 0
29 1 3 C 30 5 3 0 3 3
26 1 3 C 40 0 7 0 0 0
27 1 3 C 40 3 6 3 0 0
23 -1 3 E 50 0 0 0 7 3
".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 386 };
			gameState.projects.Add(new Project(3, 0, 0, 3, 3));
			gameState.projects.Add(new Project(0, 4, 4, 0, 0));
			gameState.projects.Add(new Project(0, 0, 0, 4, 4));
			var robotStrategy = new GatherStrategy(gameState);
			gameState.robotStrategy = robotStrategy;

			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
