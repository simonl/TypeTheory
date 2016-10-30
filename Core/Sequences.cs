using System;
using System.Collections.Generic;

namespace Core
{
    public static class Sequences
    {
        public static Sequence<T> Push<T>(this Sequence<T> sequence, T head)
        {
            return new Sequence<T>.Node(head, sequence);
        }

        public static uint IndexOf<T>(this Sequence<T> sequence, T element)
        {
            return sequence.IndexOf(selector: value => Equals(value, element) ? Match.Yes : Match.Potentially);
        }

        public static uint IndexOf<T>(this Sequence<T> sequence, Func<T, Match> selector)
        {
            uint index = 0;
            while (sequence.Tag != Sequence<T>.Tags.Empty)
            {
                var node = (Sequence<T>.Node)sequence;

                switch (selector(node.Head))
                {
                    case Match.Yes:

                        return index;
                    case Match.Potentially:

                        index++;

                        break;
                    case Match.No:
                        break;
                    default:
                        throw new InvalidProgramException("Should never happen.");
                }

                sequence = node.Tail;
            }

            throw new ArgumentException("The sequence does not contain any matching element.");
        }

        public static T GetAt<T>(this Sequence<T> sequence, uint index)
        {
            var node = (Sequence<T>.Node)sequence;

            while (index != 0)
            {
                index--;

                node = (Sequence<T>.Node)node.Tail;
            }

            return node.Head;
        }
    }
}