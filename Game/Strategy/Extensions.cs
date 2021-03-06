using System.Collections.Generic;
using System.Linq;
using Game.Types;

namespace Game.Strategy
{
	public static class Extensions
	{
	    public static List<List<Sample>> GetVariants(this List<Sample> samples)
		{
			var result = new List<List<Sample>>();
			result.Add(new List<Sample>());

			var queue = new Queue<List<Sample>>();
			foreach (var sample in samples)
				queue.Enqueue(new List<Sample> {sample});
			while (queue.Count > 0)
			{
				var current = queue.Dequeue();
				result.Add(current);
				if (current.Count < samples.Count && current.Count < Constants.MAX_TRAY)
				{
					foreach (var sample in samples)
					{
						if (!current.Contains(sample))
						{
							var newList = current.ToList();
							newList.Add(sample);
							queue.Enqueue(newList);
						}
					}
				}
			}
			return result;
		}
	}
}