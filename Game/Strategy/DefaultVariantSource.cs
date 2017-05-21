using System.Collections.Generic;
using System.Linq;
using Game.Types;

namespace Game.Strategy
{
	public class DefaultVariantSource : IVariantSource
	{
		private readonly Robot robot;

		public DefaultVariantSource(Robot robot)
		{
			this.robot = robot;
		}

		public List<Variant> GetVariants()
		{
			return robot.samples
				.Where(x => x.Diagnosed)
				.ToList().GetVariants()
				.Select(x => new Variant
				{
					samples = x,
					target = robot.target,
					eta = robot.eta
				})
				.ToList();

		}
	}
}