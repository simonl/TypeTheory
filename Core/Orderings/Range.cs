using System;

namespace Core.Orderings
{
    public sealed class Range<T>
    {
        public readonly T Start;
        public readonly T End;

        public Range(IComparer<T> comparer, T start, T end)
        {
            if (comparer.Compare(end, start) == Ordering.LessThan)
            {
                throw new ArgumentException();
            }

            Start = start;
            End = end;
        }
    }
}