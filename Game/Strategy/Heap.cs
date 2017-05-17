using System.Collections;
using System.Collections.Generic;

namespace Game.Strategy
{
	public class Heap<T> : IEnumerable<T>
	{
		private readonly Comparer<T> comparer;
		private readonly List<T> list = new List<T>();

		public Heap()
			: this(Comparer<T>.Default)
		{
		}

		public Heap(Comparer<T> comparer)
		{
			this.comparer = comparer;
		}

		public int Count => list.Count;

		public T Min => list[0];

		public void Add(T b)
		{
			list.Add(b);
			var c = list.Count - 1;
			while (c > 0)
			{
				var p = (c - 1) / 2;
				if (comparer.Compare(list[p], list[c]) <= 0)
					break;
				var t = list[p];
				list[p] = list[c];
				list[c] = t;
				c = p;
			}
		}

		public T DeleteMin()
		{
			var res = list[0];
			list[0] = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);

			var p = 0;
			while (true)
			{
				var c1 = p * 2 + 1;
				var c2 = p * 2 + 2;
				if (c1 >= list.Count)
					break;
				if (c2 >= list.Count)
				{
					if (comparer.Compare(list[p], list[c1]) > 0)
					{
						var t = list[p];
						list[p] = list[c1];
						list[c1] = t;
					}
					break;
				}
				if (comparer.Compare(list[p], list[c1]) <= 0 && comparer.Compare(list[p], list[c2]) <= 0)
					break;
				if (comparer.Compare(list[p], list[c1]) <= 0)
				{
					var t = list[p];
					list[p] = list[c2];
					list[c2] = t;
					p = c2;
				}
				else if (comparer.Compare(list[p], list[c2]) <= 0)
				{
					var t = list[p];
					list[p] = list[c1];
					list[c1] = t;
					p = c1;
				}
				else if (comparer.Compare(list[c1], list[c2]) <= 0)
				{
					var t = list[p];
					list[p] = list[c1];
					list[c1] = t;
					p = c1;
				}
				else
				{
					var t = list[p];
					list[p] = list[c2];
					list[c2] = t;
					p = c2;
				}
			}

			return res;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			return $"{nameof(Count)}: {Count}";
		}
	}
}