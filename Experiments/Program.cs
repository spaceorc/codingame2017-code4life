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
MOLECULES 0 12 1 2 1 2 0 1 0 1 0 1
DIAGNOSIS 0 31 0 0 0 0 0 1 1 1 0 0
5 4 5 4 6
6
6 0 1 E 1 2 2 0 1 0
7 0 1 B 1 1 0 2 2 0
8 0 1 E 1 1 1 1 1 0
9 1 2 C 20 0 5 3 0 0
10 1 2 E 20 0 0 0 0 5
11 1 2 D 10 0 3 0 2 3
".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 126 };
			gameState.projects.Add(new Project(0, 0, 3, 3, 3));
			gameState.projects.Add(new Project(3, 3, 0, 0, 3));
			gameState.projects.Add(new Project(3, 0, 0, 3, 3));
			var robotStrategy = new GatherStrategy(gameState);
			gameState.robotStrategy = robotStrategy;
			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
