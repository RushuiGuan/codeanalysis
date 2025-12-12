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

		/// <summary>
		/// Adds a value to the hash computation using the default equality comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to add.</typeparam>
		/// <param name="value">The value to add to the hash computation.</param>
		public void Add<T>(T value) {
			unchecked {
				if(hash == 0) { hash = Seed; }
				hash = hash * Factor + (value is null ? 0 : value.GetHashCode());
			}
		}

		/// <summary>
		/// Adds a value to the hash computation using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to add.</typeparam>
		/// <param name="value">The value to add to the hash computation.</param>
		/// <param name="comparer">The equality comparer to use for obtaining the hash code.</param>
		public void Add<T>(T value, IEqualityComparer<T> comparer) {
			unchecked {
				if(hash == 0) { hash = Seed; }
				hash = hash * Factor + (value is null ? 0 : comparer.GetHashCode(value));
			}
		}
		
		/// <summary>
		/// Returns the computed hash code. If no values have been added, returns the seed value.
		/// </summary>
		/// <returns>The computed hash code.</returns>
		public int ToHashCode() => hash == 0 ? Seed : hash;
	}
}
#endif