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
MOLECULES 0 40 0 3 2 0 0 0 2 1 0 0
SAMPLES 0 3 3 3 0 0 0 0 2 1 0 0
3 0 4 6 6
3
6 0 2 C 20 0 5 3 0 0
7 0 2 B 30 0 6 0 0 0
8 0 2 C 10 3 0 2 2 0
".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 142 };
			gameState.projects.Add(new Project(3, 3, 3, 0, 0));
			gameState.projects.Add(new Project(0, 3, 3, 3, 0));
			gameState.projects.Add(new Project(0, 0, 4, 4, 0));
			var robotStrategy = new GatherStrategy(gameState);
			gameState.robotStrategy = robotStrategy;

			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
