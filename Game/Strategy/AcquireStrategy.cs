using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class AcquireStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public AcquireStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			if (turnState.robot.eta > 0)
			{
				turnState.robot.Wait("Waiting for acquire");
				return null;
			}
			var gatherOrders = GatherOrder.GetGatherOrders(gameState, turnState, turnState.robot, new VariantSource(turnState));
			var gatherOrder = gatherOrders.SelectBestOrder(GatherOrderDefaultComparer.Build(turnState.robot));
			if (gatherOrder == null)
			{
				Console.Error.WriteLine("DOWNGRADING ACQUIRE");
				if (turnState.robot.At(ModuleType.DIAGNOSIS))
				{
					var undiagnozedSample = turnState.robot.samples.FirstOrDefault(x => !x.Diagnosed);
					if (undiagnozedSample != null)
					{
						turnState.robot.Connect(undiagnozedSample.sampleId, "DOWNGRADING ACQUIRE - analyze");
						return null;
					}
					var sampleToDrop = turnState.robot.samples.OrderByDescending(x => turnState.robot.GetHealth(gameState, x)).FirstOrDefault();
					if (sampleToDrop != null)
					{
						turnState.robot.Connect(sampleToDrop.sampleId, "DOWNGRADING ACQUIRE - drop");
						return null;
					}
					turnState.robot.GoTo(ModuleType.SAMPLES, "DOWNGRADING ACQUIRE - goto samples");
					return null;
				}
				if (turnState.robot.samples.Count == Constants.MAX_TRAY)
				{
					turnState.robot.GoTo(ModuleType.DIAGNOSIS, "DOWNGRADING ACQUIRE - goto diagnosis");
					return null;
				}
				if (!turnState.robot.At(ModuleType.SAMPLES))
				{
					turnState.robot.GoTo(ModuleType.SAMPLES, "DOWNGRADING ACQUIRE - goto samples");
					return null;
				}
				var carriedRanks = turnState.robot.samples.Select(x => x.rank).ToList();
				turnState.robot.Connect(SelectNewSampleRank(turnState, carriedRanks));
				return null;
			}

			var variant = (AcquireVariant)gatherOrder.variant;
			if (variant.ranksToCollect.Any())
			{
				if (turnState.robot.GoTo(ModuleType.SAMPLES) == GoToResult.Arrived)
					turnState.robot.Connect(variant.ranksToCollect.First());
				return null;
			}
			if (turnState.robot.samples.Count < Constants.MAX_TRAY && variant.samples.Any(s => turnState.cloudSamples.Contains(s)))
			{
				if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
					turnState.robot.Connect(variant.samples.First(s => turnState.cloudSamples.Contains(s)).sampleId);
				return null;
			}
			if (turnState.robot.samples.Any(x => !x.Diagnosed))
			{
				if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
					turnState.robot.Connect(turnState.robot.samples.First(x => !x.Diagnosed).sampleId);
				return null;
			}
			if (turnState.robot.samples.Any(s => !variant.samples.Contains(s)))
			{
				if (turnState.robot.GoTo(ModuleType.DIAGNOSIS) == GoToResult.Arrived)
					turnState.robot.Connect(turnState.robot.samples.First(s => !variant.samples.Contains(s)).sampleId);
				return null;
			}
			return new GatherStrategy(gameState);
		}

		private class AcquireVariant : Variant
		{
			public List<int> ranksToCollect;
		}

		private class VariantSource : IVariantSource
		{
			private readonly TurnState turnState;

			public VariantSource(TurnState turnState)
			{
				this.turnState = turnState;
			}

			public List<Variant> GetVariants()
			{
				var result = new List<Variant>();
				var collectableCount = Constants.MAX_TRAY - turnState.robot.samples.Count;
				var samplesCandidates = turnState.robot.samples.Where(x => x.Diagnosed).Concat(turnState.cloudSamples).ToList();
				var undiagnozedSamples = turnState.robot.samples.Where(x => !x.Diagnosed).ToList();
				foreach (var samples in samplesCandidates.GetVariants())
				{
					var robotSamples = samples.Concat(undiagnozedSamples).ToList();
					if (robotSamples.Count <= Constants.MAX_TRAY)
					{
						var variantCollectableCount = Math.Min(Constants.MAX_TRAY - robotSamples.Count, collectableCount);
						if (turnState.robot.target == ModuleType.DIAGNOSIS && samples.Any())
							variantCollectableCount = 0;
						for (var i = 0; i <= variantCollectableCount; i++)
						{
							var eta = turnState.robot.eta;
							var requireMolecules = false;
							var additionalHealth = 0;
							var target = turnState.robot.target;
							if (i != 0)
							{
								eta += Constants.distances[Tuple.Create(target, ModuleType.SAMPLES)];
								eta += i;
								eta += Constants.distances[Tuple.Create(ModuleType.SAMPLES, ModuleType.DIAGNOSIS)];
								eta += i;
								target = ModuleType.DIAGNOSIS;
								requireMolecules = true;
							}
							if (undiagnozedSamples.Count > 0)
							{
								eta += Constants.distances[Tuple.Create(target, ModuleType.DIAGNOSIS)];
								eta += undiagnozedSamples.Count;
								target = ModuleType.DIAGNOSIS;
								additionalHealth += undiagnozedSamples.Sum(x => x.rank == 3 ? 30 : x.rank == 2 ? 10 : 1);
								eta += undiagnozedSamples.Sum(
									x =>
									{
										var moleculesToGather = x.rank == 3 ? 14 : x.rank == 2 ? 8 : 5;
										var expertiseBenefit = turnState.robot.expertise.counts.Max();
										return moleculesToGather - expertiseBenefit;
									}); 
								requireMolecules = true;
							}
							var carriedRanks = robotSamples.Select(x => x.rank).ToList();
							var ranksToCollect = new List<int>();
							if (i > 0)
							{
								for (var k = 0; k < i; k++)
								{
									var rank = SelectNewSampleRank(turnState, carriedRanks);
									ranksToCollect.Add(rank);
									carriedRanks.Add(rank);
									additionalHealth += rank == 3 ? 30 : rank == 2 ? 10 : 1;
									var moleculesToGather = rank == 3 ? 14 : rank == 2 ? 8 : 5;
									var expertiseBenefit = turnState.robot.expertise.counts.Max();
									eta += moleculesToGather - expertiseBenefit;
								}
							}
							var cloudSamples = samples.Where(s => turnState.cloudSamples.Contains(s)).ToList();
							var droppedSamples = turnState.robot.samples.Where(s => s.Diagnosed && !samples.Contains(s)).ToList();
							if (cloudSamples.Count > 0 || droppedSamples.Count > 0)
							{
								eta += Constants.distances[Tuple.Create(target, ModuleType.DIAGNOSIS)];
								eta += cloudSamples.Count + droppedSamples.Count;
								target = ModuleType.DIAGNOSIS;
							}
							result.Add(new AcquireVariant
							{
								samples = samples,
								target = target,
								eta = eta,
								additionalHealth = additionalHealth,
								requireMolecules = requireMolecules,
								ranksToCollect = ranksToCollect
							});
						}
					}
				}
				return result;
			}
		}

		private static int SelectNewSampleRank(TurnState turnState, List<int> carriedRanks)
		{
			// todo try to use enemy recycling
			var sum = turnState.robot.expertise.totalCount;
			if (sum < 5)
				return 1;
			if (sum < 7)
			{
				if (carriedRanks.Count(x => x == 1) < 2)
					return 1;
				return 2;
			}
			if (sum < 9)
			{
				if (carriedRanks.Count(x => x == 1) < 1)
					return 1;
				return 2;
			}
			if (sum < 11)
			{
				if (carriedRanks.Count(x => x == 2) < 2)
					return 2;
				return 3;
			}
			if (sum < 13)
			{
				if (carriedRanks.Count(x => x == 2) < 1)
					return 2;
				return 3;
			}
			return 3;
		}
	}
}