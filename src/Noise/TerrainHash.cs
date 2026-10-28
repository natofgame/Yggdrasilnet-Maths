namespace Yggdrasilnet.Maths.Noise;

/// <summary>
///  Stateless deterministic random values for the terrain generation: the same inputs give the same value on every
///  platform and in any order of generation.
/// </summary>
public static class TerrainHash {
    /// <summary>
    ///  A value in [0, 1) for a cell of a generation pass.
    /// </summary>
    public static float Unit(int seed, int chunkX, int chunkZ, int pass, int index) {
        var hash = Mix(Mix(seed, chunkX, chunkZ) ^ (ulong)(uint)pass, index, 0);
        // The top 24 bits: exactly representable as a float.
        return (hash >> 40) * (1f / (1 << 24));
    }

    /// <summary>
    ///  Mixes three integers into 64 well-spread bits (SplitMix64 finalizer).
    /// </summary>
    public static ulong Mix(int a, int b, int c) {
        return Mix(((ulong)(uint)a << 32 | (uint)b) ^ ((ulong)(uint)c * 0x9E3779B97F4A7C15UL), b, c);
    }

    private static ulong Mix(ulong state, int b, int c) {
        state += 0x9E3779B97F4A7C15UL + (ulong)(uint)b * 0xBF58476D1CE4E5B9UL + (ulong)(uint)c;
        state = (state ^ (state >> 30)) * 0xBF58476D1CE4E5B9UL;
        state = (state ^ (state >> 27)) * 0x94D049BB133111EBUL;
        return state ^ (state >> 31);
    }
}
