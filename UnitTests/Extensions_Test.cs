using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Game.Strategy;
using Game.Types;
using NUnit.Framework;

namespace UnitTests
{
	[TestFixture]
	public class Extensions_Test
	{
		[Test]
		public void GetPermutations_EmptyList_ReturnsEmptyPermutationsList()
		{
			new List<Sample>().GetPermutations().Should().BeEmpty();
		}

		[Test]
		public void GetPermutations_OneElement_ReturnsOneElementPermutationsList()
		{
			var sample = new Sample(1, 1, 1, "A", 1, 1, 1, 1, 1, 1);
			new List<Sample> {sample}.GetPermutations()
				.ShouldBeEquivalentTo(
					new List<List<Sample>>
					{
						new List<Sample> {sample}
					});
		}

		[TestCase("AB", "AB", "BA")]
		[TestCase("ABC", "ABC", "ACB", "BAC", "BCA", "CAB", "CBA")]
		public void GetPermutations_ManyElements_ReturnsValidPermutationsList(string source, params string[] expected)
		{
			var samples = source.Select(x => new Sample(0, 0, 0, x.ToString(), 0, 0, 0, 0, 0, 0)).ToList();
			var permutations = samples.GetPermutations().Select(x => string.Join("", x.Select(i => i.gainString))).ToList();
			permutations.ShouldBeEquivalentTo(expected);
		}
	}
}