using System;

namespace Yggdrasilnet.Maths.Noise;

/// <summary>
///  2D Perlin noise, deterministic for a seed on every platform: the permutation is shuffled by
///  <see cref="TerrainHash" /> (not <see cref="System.Random" />, whose algorithm may change between runtimes).
///  Values are in [-1, 1] and 0 on the integer lattice.
/// </summary>
public sealed class PerlinNoise {
    // Eight unit directions.
    private static readonly float[] GradientX = { 1f, -1f, 0f, 0f, 0.70710678f, -0.70710678f, 0.70710678f, -0.70710678f };
    private static readonly float[] GradientZ = { 0f, 0f, 1f, -1f, 0.70710678f, 0.70710678f, -0.70710678f, -0.70710678f };

    // The largest value of 2D Perlin noise with unit gradients is sqrt(2) / 2: scaled to 1.
    private const float Scale = 1.41421356f;

    private readonly byte[] _permutation = new byte[512];

    public PerlinNoise(int seed) {
        var values = new byte[256];
        for (var i = 0; i < 256; i++) {
            values[i] = (byte)i;
        }

        for (var i = 255; i > 0; i--) {
            var j = (int)(TerrainHash.Mix(seed, i, 0x5EED) % (ulong)(i + 1));
            (values[i], values[j]) = (values[j], values[i]);
        }

        for (var i = 0; i < 512; i++) {
            _permutation[i] = values[i & 255];
        }
    }

    /// <summary>
    ///  The noise at a point.
    /// </summary>
    public float Sample(float x, float z) {
        var floorX = (int)MathF.Floor(x);
        var floorZ = (int)MathF.Floor(z);
        var fx = x - floorX;
        var fz = z - floorZ;
        var ix = floorX & 255;
        var iz = floorZ & 255;

        var g00 = Gradient(ix, iz, fx, fz);
        var g10 = Gradient(ix + 1, iz, fx - 1f, fz);
        var g01 = Gradient(ix, iz + 1, fx, fz - 1f);
        var g11 = Gradient(ix + 1, iz + 1, fx - 1f, fz - 1f);

        var u = Fade(fx);
        var v = Fade(fz);
        var value = Lerp(Lerp(g00, g10, u), Lerp(g01, g11, u), v) * Scale;
        return Math.Clamp(value, -1f, 1f);
    }

    private float Gradient(int ix, int iz, float dx, float dz) {
        var g = _permutation[_permutation[ix] + iz] & 7;
        return GradientX[g] * dx + GradientZ[g] * dz;
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;
}
