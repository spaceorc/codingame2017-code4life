using System.Collections.Generic;

namespace Game.Strategy
{
	public interface IVariantSource
	{
		List<Variant> GetVariants();
	}
}