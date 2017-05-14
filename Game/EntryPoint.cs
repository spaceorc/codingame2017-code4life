using System;
using System.IO;
using Game.State;
using Game.Types;

namespace Game
{
	public class EntryPoint
	{
		public static void MainLoop(TextReader input)
		{
			string[] inputs;
			var gameState = new GameState();
			var projectCount = int.Parse(input.ReadLine());
			for (var i = 0; i < projectCount; i++)
			{
				inputs = input.ReadLine().Split(' ');
				var a = int.Parse(inputs[0]);
				var b = int.Parse(inputs[1]);
				var c = int.Parse(inputs[2]);
				var d = int.Parse(inputs[3]);
				var e = int.Parse(inputs[4]);
				gameState.projects.Add(new Project(a, b, c, d, e));
			}

			// game loop
			while (true)
			{
				gameState.Iteration(input);
			}
		}

		private static void Main(string[] args)
		{
			MainLoop(Console.In);
		}
	}
}