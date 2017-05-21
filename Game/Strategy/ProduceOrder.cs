using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class ProduceOrder
	{
		public static IEnumerable<ProduceOrder> GetProduceOrders(GameState gameState, Robot robot, List<Sample> robotSamples = null)
		{
			robotSamples = robotSamples ?? robot.samples;
			foreach (var samples in robotSamples.Where(x => x.Diagnosed).ToList().GetVariants())
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
					yield return new ProduceOrder(
						additionalExpertise,
						usedMolecules,
						producedSamples,
						samples.Except(producedSamples.Select(x => x.sample)).ToList(),
						samples,
						eta,
						health);
				}
			}
		}

		public readonly MoleculeSet additionalExpertise;
		public readonly MoleculeSet usedMolecules;
		public readonly List<GatheredSample> producedSamples;
		public readonly List<Sample> samplesLeft;
		public readonly List<Sample> variant;
		public readonly int eta;
		public readonly int health;

		public ProduceOrder(MoleculeSet additionalExpertise, MoleculeSet usedMolecules, List<GatheredSample> producedSamples, List<Sample> samplesLeft, List<Sample> variant, int eta, int health)
		{
			this.additionalExpertise = additionalExpertise;
			this.usedMolecules = usedMolecules;
			this.producedSamples = producedSamples;
			this.samplesLeft = samplesLeft;
			this.variant = variant;
			this.eta = eta;
			this.health = health;
		}
	}
}