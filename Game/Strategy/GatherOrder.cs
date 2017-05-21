using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class GatherOrder
	{
		public MoleculeSet additionalExpertise = new MoleculeSet();
		public MoleculeSet usedMolecules = new MoleculeSet();
		public List<GatheredSample> producedSamples = new List<GatheredSample>();
		public List<GatheredSample> gatheredSamples = new List<GatheredSample>();
		public List<GatheredSample> gatheredNowSamples = new List<GatheredSample>();
		public List<GatheredSample> gatheredSoonSamples = new List<GatheredSample>();
		public List<GatheredSample> gatheredAfterRecycleSamples = new List<GatheredSample>();
		public List<Sample> samplesLeft = new List<Sample>();
		public int eta;

		public GatherOrder(GameState gameState, TurnState turnState, Robot robot)
		{
			var enemy = turnState.robots.Single(x => x != robot);
			var enemyProduceOrder = enemy.target == ModuleType.LABORATORY ? new ProduceOrder(gameState, enemy) : null;

			var maxHealth = int.MinValue;
			var maxMinExpertise = int.MinValue;
			var minMinExpertiseCount = int.MaxValue;
			samplesLeft = robot.samples.ToList();
			foreach (var samples in robot.samples.Where(x => x.Diagnosed).ToList().GetPermutations())
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
					if (gameState.currentTurn + eta * 2 <= Constants.TOTAL_TURNS)
					{
						var expertise = robot.expertise.Add(additionalExpertise);
						var minExpertise = expertise.counts.Min();
						var minExpertiseCount = expertise.counts.Count(t => t == minExpertise);
						if (health > maxHealth
						    
							|| health == maxHealth 
							&& usedMolecules.totalCount < this.usedMolecules.totalCount

						    || health == maxHealth && usedMolecules.totalCount == this.usedMolecules.totalCount 
							&& gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count() > this.gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count()

							|| health == maxHealth && usedMolecules.totalCount == this.usedMolecules.totalCount && gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count() == this.gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count()
							&& minExpertise > maxMinExpertise

						    || health == maxHealth && usedMolecules.totalCount == this.usedMolecules.totalCount && gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count() == this.gatheredSamples.TakeWhile(x => x.type == GatheredSampleType.Gathered).Count() && minExpertise == maxMinExpertise
							&& minExpertiseCount < minMinExpertiseCount)
						{
							maxHealth = health;
							maxMinExpertise = minExpertise;
							minMinExpertiseCount = minExpertiseCount;
							this.additionalExpertise = additionalExpertise;
							this.usedMolecules = usedMolecules;
							this.producedSamples = producedSamples;
							this.gatheredSamples = gatheredSamples;
							this.gatheredNowSamples = gatheredNowSamples;
							this.gatheredSoonSamples = gatheredSoonSamples;
							samplesLeft = samples
								.Except(producedSamples.Select(x => x.sample))
								.Except(gatheredSamples.Select(x => x.sample))
								.ToList();
							this.eta = eta;
							gatheredAfterRecycleSamples = samplesLeft
								.Where(x => robot.CanGather(turnState, x, additionalExpertise, usedMolecules, recycle: true, comingSoonMolecules: enemyProduceOrder?.usedMolecules))
								.Select(x => new GatheredSample(x, robot.GetHealth(gameState, x, additionalExpertise), GatheredSampleType.GatheredAfterRecycle, robot.GetMoleculesToGather(x, additionalExpertise, usedMolecules)))
								.OrderByDescending(x => x.health)
								.ToList();
						}
					}
				}
			}
		}
	}
}