using System.Collections.Generic;
using System.Linq;

namespace Game.Strategy
{
	public static class OrderExtensions
	{
		public static T SelectBestOrder<T>(this IEnumerable<T> source, IComparer<T> comparer)
		{
			return source.OrderBy(x => x, comparer).FirstOrDefault();
		}

		public static List<T> SelectBestOrders<T>(this IEnumerable<T> source, IComparer<T> comparer)
		{
			var list = source.OrderBy(x => x, comparer).ToList();
			var first = list.FirstOrDefault();
			if (first == null)
				return new List<T>();
			return list.TakeWhile(x => comparer.Compare(x, first) == 0).ToList();
		}
	}
}