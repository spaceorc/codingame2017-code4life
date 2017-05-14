using System.IO;
using Game;
using Game.State;

namespace Experiments
{
	class Program
	{
		private static void Main(string[] args)
		{
			var state = @"
SAMPLES 0 0 0 0 0 0 0 0 0 0 0 0
DIAGNOSIS 0 0 0 0 0 0 0 0 0 0 0 0
99 99 99 99 99
0
".Trim();

			Settings.DUMP_ALL = true;
			Settings.DUMP_TURN = -1;

			////===
			var gameState = new GameState { currentTurn = 44 };

			////===
			gameState.Iteration(new StringReader(state));
		}
	}
}
