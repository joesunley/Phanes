global using static Phanes.Common.Globals;
global using Sunley.Mathematics;
global using System.Diagnostics;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("Console")]

namespace Phanes.Common;


public static class Globals
{
    public static void Log(object message, int logCode = 0)
    {
        int outCode = 0;

        if (outCode == 0)
            Debug.WriteLine(message);

        else if (logCode == outCode)
            Debug.WriteLine(message);
    }

    public static double ZoomFunc(double x)
        => (0.2475 * x) - 0.2375;
}

public static class Extensions
{
    public static vec2 Nearest(this IEnumerable<vec2> points, vec2 p)
    {
        float dist = float.MaxValue;
        vec2 nearest = new();

        foreach (var point in points)
        {
            float d = vec2.Mag(point, p);

            if (d < dist)
            {
                dist = d;
                nearest = point;
            }
        }

        return nearest;
    }

    public static float Length(this IEnumerable<vec2> v2s)
    {
        // Faster iteration times using Span
        ReadOnlySpan<vec2> span = v2s.ToArray();

        float totalLength = 0f;
        for (int i = 1; i < span.Length; i++)
            totalLength += vec2.Mag(span[i - 1], span[i]);

        return totalLength;
    }

    public static float GetLength(this vec4 v4, int index) {
        switch (index)
        {
            case 0:
            {
                vec2 x = (vec2)v4;
                vec2 y = new(v4.Z, v4.Y);

                return vec2.Mag(x, y);
            }
            case 1:
            {
                vec2 x = (vec2)v4;
                vec2 y = new(v4.X, v4.W);

                return vec2.Mag(x, y);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be either 0 or 1");
        }
    }
    
    // public static bool HasControlFlag(this KeyModifiers modifiers)
    //     => modifiers.HasFlag(KeyModifiers.LeftControl) || modifiers.HasFlag(KeyModifiers.RightControl);

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
            action(item);
    }
    
    internal static byte[] Params(this byte[] a, params byte[] b)
        => a.Concat(b).ToArray();
}