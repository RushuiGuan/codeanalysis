using System.Diagnostics.CodeAnalysis;

namespace Albatross.CodeAnalysis.UnitTest {
	public class TestNetStandard2Reference {
		public bool TestConflict([NotNullWhen(true)] out string? message) {
			message = null;
			return false;
		}
	}
}