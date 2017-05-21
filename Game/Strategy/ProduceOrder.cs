using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class ProduceOrder
	{
		public MoleculeSet additionalExpertise = new MoleculeSet();
		public MoleculeSet usedMolecules = new MoleculeSet();
		public List<GatheredSample> producedSamples = new List<GatheredSample>();
		public List<Sample> samplesLeft = new List<Sample>();
		public int eta;

		public ProduceOrder(GameState gameState, Robot robot)
		{
			var maxHealth = int.MinValue;
			var maxMinExpertise = int.MinValue;
			var minMinExpertiseCount = int.MaxValue;
			samplesLeft = robot.samples.ToList();
			foreach (var samples in robot.samples.Where(x => x.Diagnosed).ToList().GetVariants())
			{
				var additionalExpertise = new MoleculeSet();
				var usedMolecules = new MoleculeSet();
				var producedSamples = new List<GatheredSample>();
				var health = 0;
				foreach (var sample in samples)
				{
					if (robot.CanProduce(sample, additionalExpertise, usedMolecules))
					{
						var additionalHealth = robot.GetHealth(gameState, sample, additionalExpertise);
						health += additionalHealth;
						usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
						additionalExpertise = additionalExpertise.Add(sample.gain);
						producedSamples.Add(new GatheredSample(sample, additionalHealth, GatheredSampleType.Produced, new MoleculeSet()));
					}
				}
				var eta = robot.eta + Constants.distances[Tuple.Create(robot.target, ModuleType.LABORATORY)] + producedSamples.Count;
				if (gameState.currentTurn + eta*2 <= Constants.TOTAL_TURNS)
				{
					var expertise = robot.expertise.Add(additionalExpertise);
					var minExpertise = expertise.counts.Min();
					var minExpertiseCount = expertise.counts.Count(t => t == minExpertise);
					if (health > maxHealth
					    || health == maxHealth && usedMolecules.totalCount < this.usedMolecules.totalCount
					    || health == maxHealth && usedMolecules.totalCount == this.usedMolecules.totalCount && minExpertise > maxMinExpertise
					    || health == maxHealth && usedMolecules.totalCount == this.usedMolecules.totalCount && minExpertise == maxMinExpertise && minExpertiseCount < minMinExpertiseCount)
					{
						maxHealth = health;
						maxMinExpertise = minExpertise;
						minMinExpertiseCount = minExpertiseCount;
						this.additionalExpertise = additionalExpertise;
						this.usedMolecules = usedMolecules;
						this.producedSamples = producedSamples;
						samplesLeft = robot.samples.Except(producedSamples.Select(x => x.sample)).ToList();
						this.eta = eta;
					}
				}
			}
		}
	}
}