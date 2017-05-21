using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class GatherOrder
	{
		public readonly MoleculeSet additionalExpertise;
		public readonly MoleculeSet usedMolecules;
		public readonly List<GatheredSample> producedSamples;
		public readonly List<GatheredSample> gatheredSamples;
		public readonly List<GatheredSample> gatheredNowSamples;
		public readonly List<GatheredSample> gatheredSoonSamples;
		public readonly List<GatheredSample> gatheredAfterRecycleSamples;
		public readonly List<Sample> samplesLeft;
		public readonly List<Sample> variant;
		public readonly int eta;
		public readonly int health;

		public static IEnumerable<GatherOrder> GetGatherOrders(GameState gameState, TurnState turnState, Robot robot, List<Sample> robotSamples = null)
		{
			var enemy = turnState.robots.Single(x => x != robot);
			var enemyProduceOrder = enemy.target == ModuleType.LABORATORY
				? ProduceOrder.GetProduceOrders(gameState, enemy).SelectBestOrder(new ProduceOrderDefaultComparer(enemy))
				: null;

			robotSamples = robotSamples ?? robot.samples;
			foreach (var samples in robotSamples.Where(x => x.Diagnosed).ToList().GetVariants())
			{
				var additionalExpertise = new MoleculeSet();
				var usedMolecules = new MoleculeSet();
				var producedSamples = new List<GatheredSample>();
				var gatheredSamples = new List<GatheredSample>();
				var gatheredNowSamples = new List<GatheredSample>();
				var gatheredSoonSamples = new List<GatheredSample>();
				var health = 0;
				var eta = 0;
				foreach (var sample in samples)
				{
					if (robot.CanProduce(sample, additionalExpertise, usedMolecules))
					{
						eta++;
						var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
						health += additionalHealth;
						usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
						additionalExpertise = additionalExpertise.Add(sample.gain);
						producedSamples.Add(new GatheredSample(sample, additionalHealth, GatheredSampleType.Produced, new MoleculeSet()));
					}
					else if (robot.CanGather(turnState, sample, additionalExpertise, usedMolecules))
					{
						var moleculesToGather = robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules);
						eta += moleculesToGather.totalCount;
						var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
						health += additionalHealth;
						usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
						additionalExpertise = additionalExpertise.Add(sample.gain);
						var gatheredSample = new GatheredSample(sample, additionalHealth, GatheredSampleType.Gathered, moleculesToGather);
						gatheredNowSamples.Add(gatheredSample);
						gatheredSamples.Add(gatheredSample);
					}
					else if (robot.CanGather(turnState, sample, additionalExpertise, usedMolecules, comingSoonMolecules: enemyProduceOrder?.usedMolecules))
					{
						var moleculesToGather = robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules);
						eta += moleculesToGather.totalCount;
						var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
						health += additionalHealth;
						usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
						additionalExpertise = additionalExpertise.Add(sample.gain);
						var gatheredSample = new GatheredSample(sample, additionalHealth, GatheredSampleType.GatheredSoon, moleculesToGather);
						gatheredSoonSamples.Add(gatheredSample);
						gatheredSamples.Add(gatheredSample);
					}
				}
				eta += robot.eta;
				if (gatheredSamples.Any())
					eta += Constants.distances[Tuple.Create(robot.target, ModuleType.MOLECULES)] + Constants.distances[Tuple.Create(ModuleType.MOLECULES, ModuleType.LABORATORY)];
				else if (producedSamples.Any())
					eta += Constants.distances[Tuple.Create(robot.target, ModuleType.LABORATORY)];
				if (gatheredSoonSamples.Any())
					eta += enemyProduceOrder?.eta ?? 0;
				if (gatheredSamples.Any() || producedSamples.Any())
				{
					if (gameState.currentTurn + eta*2 <= Constants.TOTAL_TURNS)
					{
						var samplesLeft = samples
							.Except(producedSamples.Select(x => x.sample))
							.Except(gatheredSamples.Select(x => x.sample))
							.ToList();
						var gatheredAfterRecycleSamples = samplesLeft
							.Where(x => robot.CanGather(turnState, x, additionalExpertise, usedMolecules, recycle: true, comingSoonMolecules: enemyProduceOrder?.usedMolecules))
							.Select(x => new GatheredSample(x, robot.GetHealth(gameState, x, additionalExpertise), GatheredSampleType.GatheredAfterRecycle, robot.GetMoleculesToGather(x, additionalExpertise, usedMolecules)))
							.OrderByDescending(x => x.health)
							.ToList();
						yield return new GatherOrder(
							additionalExpertise, 
							usedMolecules, 
							producedSamples, 
							gatheredSamples, 
							gatheredNowSamples, 
							gatheredSoonSamples, 
							gatheredAfterRecycleSamples,
							samplesLeft,
							samples, 
							eta, 
							health);
					}
				}
			}
		}

		public GatherOrder(MoleculeSet additionalExpertise, MoleculeSet usedMolecules, List<GatheredSample> producedSamples, List<GatheredSample> gatheredSamples, List<GatheredSample> gatheredNowSamples, List<GatheredSample> gatheredSoonSamples, List<GatheredSample> gatheredAfterRecycleSamples, List<Sample> samplesLeft, List<Sample> variant, int eta, int health)
		{
			this.additionalExpertise = additionalExpertise;
			this.usedMolecules = usedMolecules;
			this.producedSamples = producedSamples;
			this.gatheredSamples = gatheredSamples;
			this.gatheredNowSamples = gatheredNowSamples;
			this.gatheredSoonSamples = gatheredSoonSamples;
			this.gatheredAfterRecycleSamples = gatheredAfterRecycleSamples;
			this.samplesLeft = samplesLeft;
			this.variant = variant;
			this.eta = eta;
			this.health = health;
		}
	}
}