using System;
using System.Data;

namespace Core.Orderings
{
    public sealed class RecordAccess<T>
    {
        private readonly IComparer<T> Comparer;

        private readonly T[] Given;
        private readonly T Element;

        private uint Find(T[] expected, uint staticIndex)
        {
            switch (new Comparer<int>().Compare(Given.Length, expected.Length))
            {
                case Ordering.LessThan:

                    throw new StrongTypingException();
                case Ordering.EqualTo:

                    return staticIndex;
                case Ordering.GreaterThan:

                    var range = Full(Given);

                    return Find(range);
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private uint Find(Range<uint> range)
        {
            switch(Size(range))
            {
                case 0U:
                {
                    throw new StrongTypingException();
                }
                case 1U:
                {
                    return range.Start;
                }
                default:
                {
                    var middle = Mean(range);

                    var found = Given[middle];

                    var ordering = Comparer.Compare(Element, found);

                    range = Partition(range, middle, ordering);

                    return Find(range);
                }
            }
        }

        private static Range<uint> Full(T[] array)
        {
            return new Range<uint>(new Comparer<uint>(), 0, (uint)array.Length);
        }

        private static uint Size(Range<uint> range)
        {
            return range.End - range.Start;
        }

        private static uint Mean(Range<uint> range)
        {
            if (Size(range) < 2)
            {
                throw new ArgumentException();
            }

            return (range.Start + range.End) / 2;
        }

        private static Range<uint> Partition(Range<uint> range, uint middle, Ordering ordering)
        {
            switch (ordering)
            {
                case Ordering.LessThan:

                    return new Range<uint>(new Comparer<uint>(), range.Start, middle);
                case Ordering.EqualTo:

                    return new Range<uint>(new Comparer<uint>(), middle, middle + 1);
                case Ordering.GreaterThan:

                    return new Range<uint>(new Comparer<uint>(), middle + 1, range.End);
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}