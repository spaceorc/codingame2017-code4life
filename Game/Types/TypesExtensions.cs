using System.Linq;

namespace Game.Types
{
	public static class TypesExtensions
	{
		public static string ToMoleculesString(this int[] a)
		{
			return string.Join("", a.SelectMany((x, m) => Enumerable.Range(0, x).Select(i => (MoleculeType)m)));
		}
	}
}