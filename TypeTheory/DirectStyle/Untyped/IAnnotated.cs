namespace TypeTheory.DirectStyle.Untyped
{
    public interface IAnnotated<out T>
    {
        Polarity Polarity { get; }
        Classes Class { get; }
        T Content { get; }
    }

    public sealed class Annotated<T> : IAnnotated<T>
    {
        public Polarity Polarity { get; private set; }
        public Classes Class { get; private set; }
        public T Content { get; private set; }

        public Annotated(Polarity polarity, Classes @class, T content)
        {
            Content = content;
            Class = @class;
            Polarity = polarity;
        }
    }
}