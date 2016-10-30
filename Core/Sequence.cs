namespace Core
{
    public abstract class Sequence<T>
    {
        public enum Tags { Empty, Node }

        public readonly Tags Tag;

        private Sequence(Tags tag)
        {
            Tag = tag;
        }

        public sealed class Empty : Sequence<T>
        {
            public Empty() : base(Tags.Empty)
            {
                
            }
        }

        public sealed class Node : Sequence<T>
        {
            public readonly T Head;
            public readonly Sequence<T> Tail;
 
            public Node(T head, Sequence<T> tail) : base(Tags.Node)
            {
                Head = head;
                Tail = tail;
            }
        }
    }
}