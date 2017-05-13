using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game
{
	public class EntryPoint
	{
		private static void Main(string[] args)
		{
			MainLoop(Console.In);
		}

		public static void MainLoop(TextReader input)
		{
			string[] inputs;
			var line = input.ReadLine();
			Console.Error.WriteLine("--- initial");
			Console.Error.WriteLine(line);
			var projectCount = int.Parse(line);
			for (var i = 0; i < projectCount; i++)
			{
				line = input.ReadLine();
				Console.Error.WriteLine(line);
				inputs = line.Split(' ');

				var a = int.Parse(inputs[0]);
				var b = int.Parse(inputs[1]);
				var c = int.Parse(inputs[2]);
				var d = int.Parse(inputs[3]);
				var e = int.Parse(inputs[4]);
			}

			//var diagnozedSamples = new HashSet<int>();

			// game loop
			while (true)
			{
				var robots = new List<Robot>();

				Console.Error.WriteLine("--- turn");
				for (var i = 0; i < 2; i++)
				{
					line = input.ReadLine();
					Console.Error.WriteLine(line);
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
				Console.Error.WriteLine(line);
				inputs = line.Split(' ');
				var availableA = int.Parse(inputs[0]);
				var availableB = int.Parse(inputs[1]);
				var availableC = int.Parse(inputs[2]);
				var availableD = int.Parse(inputs[3]);
				var availableE = int.Parse(inputs[4]);
				line = input.ReadLine();
				Console.Error.WriteLine(line);
				var sampleCount = int.Parse(line);

				var samples = new List<Sample>();

				for (var i = 0; i < sampleCount; i++)
				{
					line = input.ReadLine();
					Console.Error.WriteLine(line);
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
				Console.Error.WriteLine("---");

				var sample = samples.FirstOrDefault(x => x.carriedBy == 0);
				if (sample == null)
				{
					if (robots[0].target == "SAMPLES" && robots[0].eta == 0)
					{
						Console.WriteLine($"CONNECT {1}");
					}
					else
					{
						Console.WriteLine("GOTO SAMPLES");
					}
				}
				else if (sample.cost[0] < 0)
				{
					if (robots[0].target == "DIAGNOSIS" && robots[0].eta == 0)
					{
						Console.WriteLine($"CONNECT {sample.sampleId}");
					}
					else
					{
						Console.WriteLine("GOTO DIAGNOSIS");
					}
				}
				else
				{
					var missingType = FindMissingType(robots[0].storage, sample.cost);
					if (missingType < 0)
					{
						if (robots[0].target == "LABORATORY" && robots[0].eta == 0)
						{
							Console.WriteLine($"CONNECT {sample.sampleId}");
						}
						else
						{
							Console.WriteLine("GOTO LABORATORY");
						}
					}
					else
					{
						if (robots[0].target == "MOLECULES" && robots[0].eta == 0)
						{
							Console.WriteLine($"CONNECT {(char)('A' + missingType)}");
						}
						else
						{
							Console.WriteLine("GOTO MOLECULES");
						}
					}
				}
			}
		}

		private static int FindMissingType(int[] storage, int[] cost)
		{
			for (int i = 0; i < cost.Length; i++)
			{
				if (cost[i] > storage[i])
					return i;
			}
			return -1;
		}
	}
}