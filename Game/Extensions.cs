namespace Game
{
	public static class Extensions
	{
		public static char ToMoleculeType(this int type)
		{
			return (char)(type + 'A');
		}
	}
}