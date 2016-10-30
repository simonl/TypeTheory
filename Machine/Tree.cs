namespace Machine
{
    public abstract class Tree<T>
    {
        public enum Tags { Leaf, Branch }

        public readonly Tags Tag;

        private Tree(Tags tag)
        {
            Tag = tag;
        }

        public sealed class Leaf
        {
            public readonly T Content;

            public Leaf(T content)
            {
                Content = content;
            }
        }

        public sealed class Branch
        {
            public readonly Tree<T> Left;
            public readonly Tree<T> Right;

            public Branch(Tree<T> left, Tree<T> right)
            {
                Left = left;
                Right = right;
            }
        }
    }
}