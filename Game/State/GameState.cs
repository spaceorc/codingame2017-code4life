using System;
using System.Collections.Generic;
using System.IO;
using Game.Strategy;
using Game.Types;

namespace Game.State
{
	public class GameState
	{
		public int currentTurn;
		public readonly List<Project> projects = new List<Project>();
		public IRobotStrategy robotStrategy;

		public GameState()
		{
			robotStrategy = new InitialStrategy(this);
		}

		public void Dump()
		{
			Console.Error.WriteLine($"var gameState = new {nameof(GameState)} {{ {nameof(currentTurn)} = {currentTurn} }};");
			foreach (var project in projects)
				Console.Error.WriteLine($"gameState.projects.Add({project.Dump()});");
			robotStrategy.Dump("gameState");

		}

		public void Iteration(TextReader input)
		{
			currentTurn += 2;
			var turnState = TurnState.ReadFrom(input);
			Console.Error.WriteLine("Current turn: " + currentTurn);
			if (currentTurn == Settings.DUMP_TURN || Settings.DUMP_ALL)
			{
				turnState.WriteTo(Console.Error);
				Console.Error.WriteLine("===");
				Dump();
			}

			turnState.stopwatch.Restart();

			while (true)
			{
				var newStrategy = robotStrategy.Process(turnState);
				if (newStrategy == null)
					break;
				Console.Error.WriteLine($"New strategy: {newStrategy.GetType().Name}");
				robotStrategy = newStrategy;
			}

			turnState.stopwatch.Stop();
			Console.Error.WriteLine($"Decision made in {turnState.stopwatch.ElapsedMilliseconds} ms");
		}
	}
}