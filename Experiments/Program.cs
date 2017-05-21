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
MOLECULES 0 0 0 1 0 0 0 0 0 0 0 0
MOLECULES 0 0 0 1 0 0 0 0 0 0 0 0
5 3 5 5 5
6
1 0 1 C 1 0 1 0 2 2
3 0 1 A 1 3 1 0 0 1
5 0 1 E 1 1 2 1 1 0
0 1 1 E 1 0 0 3 0 0
2 1 1 A 1 0 3 0 0 0
4 1 1 E 10 0 4 0 0 0

".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 32 };
			gameState.projects.Add(new Project(0, 0, 4, 4, 0));
			gameState.projects.Add(new Project(4, 0, 0, 0, 4));
			gameState.projects.Add(new Project(3, 3, 0, 0, 3));
			var robotStrategy = new GatherStrategy(gameState);
			gameState.robotStrategy = robotStrategy;
			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
