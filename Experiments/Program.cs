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
MOLECULES 0 3 4 0 0 0 2 0 1 2 0 0
MOLECULES 0 3 1 1 2 4 0 0 1 0 1 1
0 4 3 1 3
6
8 0 1 C 1 2 1 0 0 0
10 0 1 C 1 1 1 0 1 2
11 0 1 D 1 3 0 0 0 0
6 1 1 B 10 0 0 0 4 0
7 1 1 E 1 2 0 2 0 0
9 1 1 A 1 0 0 0 2 1

".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 102 };
			gameState.projects.Add(new Project(0, 0, 4, 4, 0));
			gameState.projects.Add(new Project(0, 4, 4, 0, 0));
			gameState.projects.Add(new Project(0, 3, 3, 3, 0));
			var robotStrategy = new GatherStrategy(gameState);
			gameState.robotStrategy = robotStrategy;
			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
