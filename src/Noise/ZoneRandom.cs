namespace Yggdrasilnet.Maths.Noise;

/// <summary>
///  A deterministic random generator (SplitMix64) for the map generation: the same seed gives the same values on
///  every platform. <see cref="Fork" /> gives each generation step its own stream, so a change in one step does not
///  shift the values of the others.
/// </summary>
public sealed class ZoneRandom {
    private ulong _state;

    public ZoneRandom(ulong seed) => _state = seed;

    /// <summary>
    ///  An independent generator for the given stream.
    /// </summary>
    public ZoneRandom Fork(int stream) => new(TerrainHash.Mix((int)_state, (int)(_state >> 32), stream));

    /// <summary>
    ///  A value in [0, 1).
    /// </summary>
    public float NextFloat() => (Next() >> 40) * (1f / (1 << 24));

    /// <summary>
    ///  A value in [<paramref name="minInclusive" />, <paramref name="maxInclusive" />].
    /// </summary>
    public int NextInt(int minInclusive, int maxInclusive) {
        if (maxInclusive <= minInclusive) {
            return minInclusive;
        }

        var range = (ulong)((long)maxInclusive - minInclusive + 1);
        return (int)((long)minInclusive + (long)(Next() % range));
    }

    /// <summary>
    ///  A value in [<paramref name="min" />, <paramref name="max" />).
    /// </summary>
    public float NextRange(float min, float max) => min + (max - min) * NextFloat();

    private ulong Next() {
        _state += 0x9E3779B97F4A7C15UL;
        var z = _state;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }
}
