using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    static class Condition
    {
        public static Func<long, bool> EqualTo(long threshold) => (long currentValue) => currentValue == threshold;
        public static Func<long, bool> LessThan(long threshold) => (long currentValue) => currentValue < threshold;
        public static Func<long, bool> LessThanOrEqualTo(long threshold) => (long currentValue) => currentValue <= threshold;
        public static Func<long, bool> GreaterThan(long threshold) => (long currentValue) => currentValue > threshold;
        public static Func<long, bool> GreaterThanOrEqualTo(long threshold) => (long currentValue) => currentValue >= threshold;

        public static Func<double, bool> LessThan(double threshold) => (double currentValue) => currentValue < threshold;
        public static Func<double, bool> GreaterThan(double threshold) => (double currentValue) => currentValue > threshold;
    }
}