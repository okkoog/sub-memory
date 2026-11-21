using System;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class Rasterizer
    {
        private static int Round(float value) => (int)Math.Round(value);
        private static int Floor(float value) => (int)Math.Floor(value);
        private static int Ceil(float value) => (int)Math.Ceiling(value);
        private sealed class Axis
        {
            internal int[] Start, End;
            internal float[] First, Last;
            internal float Span;
            internal Axis(int origin, int length, int destination)
            {
                Start = new int[destination]; End = new int[destination];
                First = new float[destination]; Last = new float[destination];
                Span = (float)length / destination;
                for (int i = 0; i < destination; i++)
                {
                    float a = origin + (float)i * length / destination, b = origin + (float)(i + 1) * length / destination;
                    Start[i] = Floor(a); End[i] = Ceil(b) - 1;
                    First[i] = Math.Min(Start[i] + 1, b) - a;
                    Last[i] = b - Math.Max(End[i], a);
                }
            }
        }

        internal static Color[] FitPixels(Color32[] pixels, int w, int h, int size)
            => Resample(pixels, w, h, size, true);

        internal static Color[] ResizeCanvas(Color32[] pixels, int w, int h, int size)
            => Resample(pixels, w, h, size, false);

        private static Color[] Resample(Color32[] pixels, int w, int h, int size, bool trim)
        {
                int left = w, bottom = h, right = -1, top = -1;
                if (!trim) { left = 0; bottom = 0; right = w - 1; top = h - 1; }
                else
                for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
                    if (pixels[y * w + x].a > 12)
                    { left = Math.Min(left, x); right = Math.Max(right, x); bottom = Math.Min(bottom, y); top = Math.Max(top, y); }
                if (right < left) return null;
                int cw = right - left + 1, ch = top - bottom + 1;
                float scale = Math.Min((float)size / cw, (float)size / ch);
                int dw = Math.Max(1, Round(cw * scale)), dh = Math.Max(1, Round(ch * scale));
                var output = new Color[size * size];
                int ox = (size - dw) / 2, oy = (size - dh) / 2;
                var xs = new Axis(left, cw, dw);
                var ys = new Axis(bottom, ch, dh);
                // Area filtering is separable. Reuse horizontal row results for
                // overlapping destination rows, and calculate coverage only once
                // per axis instead of for every source pixel of every output.
                var row = new Color[dw];
                var sums = new Color[dw];
                int cachedRow = -1;
                const float byteScale = 1f / 255f;
                for (int y = 0; y < dh; y++)
                {
                    Array.Clear(sums, 0, dw);
                    for (int sy = ys.Start[y]; sy <= ys.End[y]; sy++)
                    {
                        if (cachedRow != sy)
                        {
                            for (int x = 0; x < dw; x++)
                            {
                                float r = 0, g = 0, b = 0, a = 0;
                                for (int sx = xs.Start[x]; sx <= xs.End[x]; sx++)
                                {
                                    float q = sx == xs.Start[x] ? xs.First[x] : sx == xs.End[x] ? xs.Last[x] : 1;
                                    var p = pixels[sy * w + sx];
                                    float alpha = p.a * q;
                                    r += p.r * alpha; g += p.g * alpha; b += p.b * alpha; a += alpha;
                                }
                                row[x] = new Color(r * byteScale * byteScale, g * byteScale * byteScale,
                                    b * byteScale * byteScale, a * byteScale);
                            }
                            cachedRow = sy;
                        }
                        float vertical = sy == ys.Start[y] ? ys.First[y] : sy == ys.End[y] ? ys.Last[y] : 1;
                        for (int x = 0; x < dw; x++)
                        {
                            var p = row[x];
                            sums[x].r += p.r * vertical; sums[x].g += p.g * vertical;
                            sums[x].b += p.b * vertical; sums[x].a += p.a * vertical;
                        }
                    }
                    for (int x = 0; x < dw; x++)
                    {
                        var p = sums[x];
                        output[(oy + y) * size + ox + x] = p.a > 0 ?
                            new Color(p.r / p.a, p.g / p.a, p.b / p.a, p.a / (xs.Span * ys.Span)) : Color.clear;
                    }
                }
                return output;
        }
    }
}

