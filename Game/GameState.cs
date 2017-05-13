using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
	public class GameState
	{
		public int currentTurn;
		public readonly List<Project> projects = new List<Project>();
		public readonly Strategy strategy;

		public GameState()
		{
			strategy = new Strategy(this);
		}

		public void Dump()
		{
			Console.Error.WriteLine($"var gameState = new {nameof(GameState)} {{ {nameof(currentTurn)} = {currentTurn} }};");
			foreach (var project in projects)
				Console.Error.WriteLine($"gameState.projects.Add({project.Dump()});");
			strategy.Dump($"gameState.{nameof(strategy)}");

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

			strategy.Iteration(turnState);

			turnState.stopwatch.Stop();
			Console.Error.WriteLine($"Decision made in {turnState.stopwatch.ElapsedMilliseconds} ms");
		}
	}
}