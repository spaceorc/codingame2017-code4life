using System.Collections.Generic;
using Game.Types;

namespace Game.Strategy
{
	public class Variant
	{
		public List<Sample> samples;
		public ModuleType target;
		public int eta;
		public int additionalHealth;
		public bool requireMolecules;
	}
}