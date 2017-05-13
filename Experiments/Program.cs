using System.IO;
using Game;

namespace Experiments
{
	class Program
	{
		private static void Main(string[] args)
		{
			var state = @"
0
SAMPLES 0 0 0 0 0 0 0 0 0 0 0 0
DIAGNOSIS 0 0 0 0 0 0 0 0 0 0 0 0
99 99 99 99 99
0
".Trim();

			////===
			//var gameState = new GameState { currentTurn = 44 };
			//gameState.cannoneers[0] = new Cannoneer(0, gameState) { cooldown = true };
			//gameState.cannoneers[2] = new Cannoneer(2, gameState) { cooldown = false };
			//gameState.miners[0] = new Miner(0, gameState) { cooldown = 0 };
			//gameState.miners[2] = new Miner(2, gameState) { cooldown = 0 };
			//gameState.navigators[0] = new Navigator(0, gameState);
			//gameState.navigators[2] = new Navigator(2, gameState);
			//((Strateg)gameState.strateg).decisions[2] = new StrategicDecision { role = StrategicRole.Collector, targetBarrelId = 29, fireToCoord = 198, explicitCommand = null, targetCoord = 210 };
			//((Strateg)gameState.strateg).decisions[0] = new StrategicDecision { role = StrategicRole.Collector, targetBarrelId = 29, fireToCoord = 530, explicitCommand = null, targetCoord = 210 };

			////===

			EntryPoint.MainLoop(new StringReader(state));
			//gameState.Iteration(new StringReader(state));
		}
	}
}
