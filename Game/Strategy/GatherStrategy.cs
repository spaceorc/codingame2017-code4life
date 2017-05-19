using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class ProduceOrderer
	{
		public MoleculeSet additionalExpertise = new MoleculeSet();
		public MoleculeSet usedMolecules = new MoleculeSet();
		public List<Sample> producedSamples = new List<Sample>();
		public List<Sample> samples = new List<Sample>();

		public ProduceOrderer(Robot robot)
		{
			var maxHealth = int.MinValue;
			var maxMinExpertise = int.MinValue;
			var minMinExpertiseCount = int.MaxValue;
			foreach (var samples in robot.samples.Where(x => x.Diagnosed).ToList().GetPermutations())
			{
				var additionalExpertise = new MoleculeSet();
				var usedMolecules = new MoleculeSet();
				var producedSamples = new List<Sample>();
				foreach (var sample in samples)
				{
					if (robot.CanProduce(sample, additionalExpertise, usedMolecules))
					{
						usedMolecules = usedMolecules.Add(robot.GetCost(sample, additionalExpertise));
						additionalExpertise = additionalExpertise.Add(sample.gain);
						producedSamples.Add(sample);
					}
				}
				var health = producedSamples.Sum(x => x.health);
				var expertise = robot.expertise.Add(additionalExpertise);
				var minExpertise = expertise.counts.Min();
				var minExpertiseCount = expertise.counts.Count(t => t == minExpertise);
				if (health > maxHealth
					|| health == maxHealth && minExpertise > maxMinExpertise
					|| health == maxHealth && minExpertise == maxMinExpertise && minExpertiseCount < minMinExpertiseCount)
				{
					maxHealth = health;
					maxMinExpertise = minExpertise;
					minMinExpertiseCount = minExpertiseCount;
					this.additionalExpertise = additionalExpertise;
					this.usedMolecules = usedMolecules;
					this.producedSamples = producedSamples;
					this.samples = samples.Except(this.producedSamples).ToList();
				}
			}
		}
	}

	public class GatherStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public GatherStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			//var eadditionalExpertise = new MoleculeSet();
			//var eusedMolecules = new MoleculeSet();

			//var esamples = turnState.enemy.samples.ToList();

			//var eproducedSamples = new List<Sample>();
			//while (true)
			//{
			//	var canProduceSample = esamples.FirstOrDefault(x => turnState.enemy.CanProduce(x, eadditionalExpertise, eusedMolecules));
			//	if (canProduceSample == null)
			//		break;
			//	eusedMolecules = eusedMolecules.Add(turnState.enemy.GetCost(canProduceSample, eadditionalExpertise));
			//	eadditionalExpertise = eadditionalExpertise.Add(canProduceSample.gain);
			//	esamples = esamples.Except(new[] { canProduceSample }).ToList();
			//	eproducedSamples.Add(canProduceSample);
			//}





			var additionalExpertise = new MoleculeSet();
			var usedMolecules = new MoleculeSet();

			var samples = turnState.robot.samples.ToList();

			var producedSamples = new List<Sample>();
			while (true)
			{
				var canProduceSample = samples.FirstOrDefault(x => turnState.robot.CanProduce(x, additionalExpertise, usedMolecules));
				if (canProduceSample == null)
					break;
				usedMolecules = usedMolecules.Add(turnState.robot.GetCost(canProduceSample, additionalExpertise));
				additionalExpertise = additionalExpertise.Add(canProduceSample.gain);
				samples = samples.Except(new[]{ canProduceSample }).ToList();
				producedSamples.Add(canProduceSample);
			}

			//var turnsToProduce = producedSamples.Count + turnState.robot.eta + Constants.distances[Tuple.Create(turnState.robot.target, ModuleType.LABORATORY)];
			
			var canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, usedMolecules));
			if (canGatherSample != null)
			{
				var turnsToGatherAndProduce = (producedSamples.Count + turnState.robot.eta + Constants.distances[Tuple.Create(turnState.robot.target, ModuleType.MOLECULES)]
				                               + turnState.robot.GetMoleculesToGather(canGatherSample, additionalExpertise, usedMolecules).totalCount + Constants.distances[Tuple.Create(turnState.robot.target, ModuleType.LABORATORY)]
				                               + 1)*2;
				if (gameState.currentTurn + turnsToGatherAndProduce > Constants.TOTAL_TURNS && producedSamples.Any())
					return new ProduceStrategy(gameState);

				if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.OnTheWay)
					return null;
				var myMoleculesToGather = turnState.robot.GetMoleculesToGather(canGatherSample, additionalExpertise, usedMolecules);
				var enemySamples = turnState.enemy.samples.Where(x => x.Diagnosed && !turnState.enemy.CanProduce(x) && turnState.enemy.CanGather(turnState, x)).OrderByDescending(x => x.health).ToList();
				foreach (var enemySample in enemySamples)
				{
					var enemyMoleculesToGather = turnState.enemy.GetMoleculesToGather(enemySample);
					var totalMoleculesToGather = myMoleculesToGather.Add(enemyMoleculesToGather);
					var overlayMolecules = totalMoleculesToGather.Subtract(turnState.available);
					var maxi = -1;
					var max = 0;
					for (int i = 0; i < Constants.MOLECULE_TYPE_COUNT; i++)
					{
						if (overlayMolecules.counts[i] > max && myMoleculesToGather.counts[i] > 0)
						{
							max = overlayMolecules.counts[i];
							maxi = i;
						}
					}
					if (maxi != -1)
					{
						turnState.robot.Connect((MoleculeType)maxi);
						return null;
					}
				}
				turnState.robot.Connect(GetMoleculeToGather(turnState, canGatherSample, additionalExpertise, usedMolecules));
				return null;
			}

			if (turnState.robot.storage.totalCount < Constants.MAX_STORAGE)
			{
				if (turnState.robot.target == ModuleType.MOLECULES && turnState.robot.eta == 0)
				{
					var enemySamples = turnState.enemy.samples.Where(x => x.Diagnosed && !turnState.enemy.CanProduce(x) && turnState.enemy.CanGather(turnState, x)).OrderByDescending(x => x.health).ToList();
					foreach (var enemySample in enemySamples)
					{
						var moleculesToGather = turnState.enemy.GetMoleculesToGather(enemySample);
						var restMolecules = turnState.available.Subtract(moleculesToGather);
						var mini = -1;
						var min = int.MaxValue;
						for (int i = 0; i < Constants.MOLECULE_TYPE_COUNT; i++)
						{
							if (moleculesToGather.counts[i] > 0)
							{
								var need = restMolecules.counts[i] + 1;
								if (need < min)
								{
									mini = i;
									min = need;
								}
							}
						}
						if (mini != -1 && turnState.robot.storage.totalCount + min < Constants.MAX_STORAGE)
						{
							turnState.robot.Connect((MoleculeType)mini);
							return null;
						}
					}
				}

				canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, usedMolecules, recycle: true));
				if (canGatherSample != null)
				{
					var ungatheredType = GetMoleculeToGather(turnState, canGatherSample, additionalExpertise, usedMolecules);
					if (ungatheredType != MoleculeType.Unknown)
					{
						if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.Arrived)
							turnState.robot.Connect(ungatheredType);
						return null;
					}
				}
			}

			if (producedSamples.Any())
				return new ProduceStrategy(gameState);

			return new DropStrategy(gameState);
		}

		private static MoleculeType GetMoleculeToGather(TurnState turnState, Sample sample, MoleculeSet additionalExpertise = null, MoleculeSet usedMolecules = null)
		{
			var moleculesToGather = turnState.robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules);
			var minAvailable = int.MaxValue;
			var result = MoleculeType.Unknown;
			for (var i = 0; i < moleculesToGather.counts.Length; i++)
			{
				if (moleculesToGather.counts[i] > 0)
				{
					if (turnState.available.counts[i] < minAvailable && turnState.available.counts[i] > 0)
					{
						result = (MoleculeType)i;
						minAvailable = turnState.available.counts[i];
					}
				}
			}
			return result;
		}
	}
}