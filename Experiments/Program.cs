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
LABORATORY 0 3 0 3 0 0 0 0 1 0 1 1
SAMPLES 2 2 3 2 2 0 1 1 0 1 0 0
2 0 3 5 4
1
4 1 1 C 1 1 3 1 0 0

".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 82 };
			gameState.projects.Add(new Project(0, 4, 4, 0, 0));
			gameState.projects.Add(new Project(0, 0, 4, 4, 0));
			gameState.projects.Add(new Project(0, 0, 0, 4, 4));
			var robotStrategy = new ProduceStrategy(gameState);
			gameState.robotStrategy = robotStrategy;
			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
