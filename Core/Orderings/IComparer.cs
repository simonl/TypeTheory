using System;

namespace Core.Orderings
{
    public interface IComparer<in T>
    {
        Ordering Compare(T first, T second);
    }

    public sealed class Comparer<T> : IComparer<T>
        where T : IComparable<T>
    {
        public Ordering Compare(T first, T second)
        {
            var ordering = first.CompareTo(second);

            if (ordering == 0)
            {
                return Ordering.EqualTo;
            }

            if (ordering < 0)
            {
                return Ordering.LessThan;
            }

            if (0 < ordering)
            {
                return Ordering.GreaterThan;
            }

            throw new InvalidProgramException("Should never happen.");
        }
    }
}