using System;
using System.Diagnostics;

namespace ItemPortraitHD
{
    internal static class PerformanceTrace
    {
        internal readonly struct Scope : IDisposable
        {
            private readonly string stage;
            private readonly long started;
            internal Scope(string stage) { this.stage = stage; started = Stopwatch.GetTimestamp(); }
            public void Dispose()
            {
                double ms = (Stopwatch.GetTimestamp() - started) * 1000.0 / Stopwatch.Frequency;
                if (ms >= 50) UnityEngine.Debug.Log("[ItemPortraitHD][性能] " + stage + " " +
                    ms.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "ms");
            }
        }
        internal static Scope Measure(string stage) => new Scope(stage);
    }
}
