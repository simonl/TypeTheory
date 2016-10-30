namespace Core.Options
{
    public abstract class Option<T>
    {
        public enum Tags { None, Some }

        public readonly Tags Tag;

        private Option(Tags tag)
        {
            Tag = tag;
        }

        public sealed class None : Option<T>
        {
            public readonly Unit Content;

            public None(Unit content) : base(Tags.None)
            {
                Content = content;
            }
        }

        public sealed class Some : Option<T>
        {
            public readonly T Content;

            public Some(T content) : base(Tags.Some)
            {
                Content = content;
            }
        }
    }
}