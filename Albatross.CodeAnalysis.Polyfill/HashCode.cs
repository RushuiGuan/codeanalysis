#if NETSTANDARD2_0
using System.Collections.Generic;

namespace System {
	/// <summary>
	/// Minimal backport of System.HashCode for netstandard2.0.
	/// Deterministic, value-object friendly, and sufficient for record hashing.
	/// </summary>
	public struct HashCode {
		private const int Seed = 17;
		private const int Factor = 31;
		private int hash;

		public void Add<T>(T value) {
			unchecked {
				if(hash == 0) { hash = Seed; }
				hash = hash * Factor + (value is null ? 0 : value.GetHashCode());
			}
		}

		public void Add<T>(T value, IEqualityComparer<T> comparer) {
			unchecked {
				if(hash == 0) { hash = Seed; }
				hash = hash * Factor + (value is null ? 0 : comparer.GetHashCode(value));
			}
		}
		public int ToHashCode() => hash == 0 ? Seed : hash;
	}
}
#endif