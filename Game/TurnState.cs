using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Game
{
	public class TurnState
	{
		public readonly List<string> lines = new List<string>();
		public readonly List<Robot> robots = new List<Robot>();
		public readonly List<Sample> samples = new List<Sample>();
		public readonly int[] available;

		public readonly Stopwatch stopwatch = Stopwatch.StartNew();

		public static TurnState ReadFrom(TextReader input)
		{
			return new TurnState(input);
		}

		public void WriteTo(TextWriter output)
		{
			if (lines.Any())
			{
				foreach (var line in lines)
					output.WriteLine(line);
			}
			else
			{
				foreach (var robot in robots)
					output.WriteLine($"{robot.target} {robot.eta} {robot.score} {robot.storage[0]} {robot.storage[1]} {robot.storage[2]} {robot.storage[3]} {robot.storage[4]} {robot.expertise[0]} {robot.expertise[1]} {robot.expertise[2]} {robot.expertise[3]} {robot.expertise[4]}");
				output.WriteLine($"{available[0]} {available[1]} {available[2]} {available[3]} {available[4]}");
				output.WriteLine($"{samples.Count}");
				foreach (var sample in samples)
					output.WriteLine($"{sample.sampleId} {sample.carriedBy} {sample.rank} {sample.gainString} {sample.health} {sample.cost[0]} {sample.cost[1]} {sample.cost[2]} {sample.cost[3]} {sample.cost[4]}");
			}
		}

		private TurnState(TextReader input)
		{
			string line;
			string[] inputs;
			for (var i = 0; i < 2; i++)
			{
				line = input.ReadLine();
				lines.Add(line);
				inputs = line.Split(' ');
				var target = inputs[0];
				var eta = int.Parse(inputs[1]);
				var score = int.Parse(inputs[2]);
				var storageA = int.Parse(inputs[3]);
				var storageB = int.Parse(inputs[4]);
				var storageC = int.Parse(inputs[5]);
				var storageD = int.Parse(inputs[6]);
				var storageE = int.Parse(inputs[7]);
				var expertiseA = int.Parse(inputs[8]);
				var expertiseB = int.Parse(inputs[9]);
				var expertiseC = int.Parse(inputs[10]);
				var expertiseD = int.Parse(inputs[11]);
				var expertiseE = int.Parse(inputs[12]);
				robots.Add(new Robot(target, eta, score, storageA, storageB, storageC, storageD, storageE, expertiseA, expertiseB, expertiseC, expertiseD, expertiseE));
			}
			line = input.ReadLine();
			lines.Add(line);
			inputs = line.Split(' ');
			var availableA = int.Parse(inputs[0]);
			var availableB = int.Parse(inputs[1]);
			var availableC = int.Parse(inputs[2]);
			var availableD = int.Parse(inputs[3]);
			var availableE = int.Parse(inputs[4]);
			available = new[] {availableA, availableB, availableC, availableD, availableE};
			line = input.ReadLine();
			lines.Add(line);
			var sampleCount = int.Parse(line);
			for (var i = 0; i < sampleCount; i++)
			{
				line = input.ReadLine();
				lines.Add(line);
				inputs = line.Split(' ');
				var sampleId = int.Parse(inputs[0]);
				var carriedBy = int.Parse(inputs[1]);
				var rank = int.Parse(inputs[2]);
				var expertiseGain = inputs[3];
				var health = int.Parse(inputs[4]);
				var costA = int.Parse(inputs[5]);
				var costB = int.Parse(inputs[6]);
				var costC = int.Parse(inputs[7]);
				var costD = int.Parse(inputs[8]);
				var costE = int.Parse(inputs[9]);

				samples.Add(new Sample(sampleId, carriedBy, rank, expertiseGain, health, costA, costB, costC, costD, costE));
			}
		}
	}
}