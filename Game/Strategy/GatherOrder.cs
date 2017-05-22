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
		public readonly MoleculeSet acquiredMolecules;
		public readonly List<GatheredSample> producedSamples;
		public readonly List<GatheredSample> gatheredSamples;
		public readonly List<GatheredSample> gatheredNowSamples;
		public readonly List<GatheredSample> gatheredSoonSamples;
		public readonly List<Sample> samplesLeft;
		public readonly Variant variant;
		public readonly int eta;
		public readonly int health;

		public static IEnumerable<GatherOrder> GetGatherOrders(GameState gameState, TurnState turnState, Robot robot, IVariantSource variantSource = null)
		{
			var enemy = turnState.robots.Single(x => x != robot);
			var enemyProduceOrder = enemy.target == ModuleType.LABORATORY
				? ProduceOrder.GetProduceOrders(gameState, enemy).SelectBestOrder(new ProduceOrderDefaultComparer(enemy))
				: null;

			variantSource = variantSource ?? new DefaultVariantSource(robot);
			foreach (var variant in variantSource.GetVariants())
			{
				var samples = variant.samples;
				var additionalExpertise = new MoleculeSet();
				var usedMolecules = new MoleculeSet();
				var acquiredMolecules = new MoleculeSet();
				var producedSamples = new List<GatheredSample>();
				var gatheredSamples = new List<GatheredSample>();
				var gatheredNowSamples = new List<GatheredSample>();
				var gatheredSoonSamples = new List<GatheredSample>();
				var recycledMolecules = new MoleculeSet();
				var health = 0;
				var eta = variant.eta;
				var target = variant.target;
				var requireMolecules = variant.requireMolecules;
				while(samples.Any())
				{
					var gathered = false;
					var gatheredSoon = false;
					var produced = false;
					foreach (var sample in samples)
					{
						if (robot.CanProduce(sample, additionalExpertise, usedMolecules, acquiredMolecules, recycledMolecules: recycledMolecules))
						{
							eta++;
							var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
							health += additionalHealth;
							usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
							additionalExpertise = additionalExpertise.Add(sample.gain);
							producedSamples.Add(new GatheredSample(sample, additionalHealth, recycledMolecules.totalCount == 0 ? GatheredSampleType.Produced : GatheredSampleType.ProducedAfterRecycle, new MoleculeSet()));
							produced = true;
						}
						else if (robot.CanGather(turnState, sample, additionalExpertise, usedMolecules, acquiredMolecules, recycledMolecules: recycledMolecules))
						{
							var moleculesToGather = robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules, acquiredMolecules, recycledMolecules);
							acquiredMolecules = acquiredMolecules.Add(moleculesToGather);
							eta += moleculesToGather.totalCount;
							var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
							health += additionalHealth;
							usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
							additionalExpertise = additionalExpertise.Add(sample.gain);
							var gatheredSample = new GatheredSample(sample, additionalHealth, recycledMolecules.totalCount == 0 ? GatheredSampleType.Gathered : GatheredSampleType.GatheredAfterRecycle, moleculesToGather);
							gatheredNowSamples.Add(gatheredSample);
							gatheredSamples.Add(gatheredSample);
							gathered = true;
						}
						else if (robot.CanGather(turnState, sample, additionalExpertise, usedMolecules, acquiredMolecules, comingSoonMolecules: enemyProduceOrder?.usedMolecules, recycledMolecules: recycledMolecules))
						{
							var moleculesToGather = robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules, acquiredMolecules, recycledMolecules);
							acquiredMolecules = acquiredMolecules.Add(moleculesToGather);
							eta += moleculesToGather.totalCount;
							var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
							health += additionalHealth;
							usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
							additionalExpertise = additionalExpertise.Add(sample.gain);
							var gatheredSample = new GatheredSample(sample, additionalHealth, recycledMolecules.totalCount == 0 ? GatheredSampleType.GatheredAfterRecycle : GatheredSampleType.GatheredSoonAfterRecycle, moleculesToGather);
							gatheredSoonSamples.Add(gatheredSample);
							gatheredSamples.Add(gatheredSample);
							gatheredSoon = true;
						}
					}
					samples = samples
						.Except(producedSamples.Select(x => x.sample))
						.Except(gatheredSamples.Select(x => x.sample))
						.ToList();
					recycledMolecules = new MoleculeSet().Add(usedMolecules);
					if (gathered || gatheredSoon)
						eta += Constants.distances[Tuple.Create(target, ModuleType.MOLECULES)] + Constants.distances[Tuple.Create(ModuleType.MOLECULES, ModuleType.LABORATORY)];
					else if (produced)
						eta += Constants.distances[Tuple.Create(target, ModuleType.LABORATORY)];
					else
					{
						if (requireMolecules)
							eta += Constants.distances[Tuple.Create(target, ModuleType.MOLECULES)] + Constants.distances[Tuple.Create(ModuleType.MOLECULES, ModuleType.LABORATORY)];
						break;
					}
					target = ModuleType.LABORATORY;
					requireMolecules = false;
				}
				if (gatheredSamples.Any() || producedSamples.Any() || variant.additionalHealth > 0)
				{
					if (gatheredSoonSamples.Any())
						eta += enemyProduceOrder?.eta ?? 0;
					if (gameState.currentTurn + eta*2 <= Constants.TOTAL_TURNS)
					{
						yield return new GatherOrder(
							additionalExpertise, 
							usedMolecules, 
							acquiredMolecules,
							producedSamples, 
							gatheredSamples, 
							gatheredNowSamples, 
							gatheredSoonSamples, 
							samples,
							variant,
							eta, 
							health);
					}
				}
			}
		}

		public GatherOrder(MoleculeSet additionalExpertise, MoleculeSet usedMolecules, MoleculeSet acquiredMolecules, List<GatheredSample> producedSamples, List<GatheredSample> gatheredSamples, List<GatheredSample> gatheredNowSamples, List<GatheredSample> gatheredSoonSamples, List<Sample> samplesLeft, Variant variant, int eta, int health)
		{
			this.additionalExpertise = additionalExpertise;
			this.usedMolecules = usedMolecules;
			this.acquiredMolecules = acquiredMolecules;
			this.producedSamples = producedSamples;
			this.gatheredSamples = gatheredSamples;
			this.gatheredNowSamples = gatheredNowSamples;
			this.gatheredSoonSamples = gatheredSoonSamples;
			this.samplesLeft = samplesLeft;
			this.variant = variant;
			this.eta = eta;
			this.health = health;
		}
	}
}