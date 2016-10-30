namespace TypeTheory.DirectStyle
{
    public interface IExpression<Bind, Id, out T>
    {
        IUniverse Universe { get; }
        Term<Bind, Id> Type { get; }
        T Term { get; }
    }

    public sealed class Expression<Bind, Id, T> : IExpression<Bind, Id, T>
    {
        public IUniverse Universe { get; private set; }
        public Term<Bind, Id> Type { get; private set; }
        public T Term { get; private set; }

        public Expression(IUniverse universe, Term<Bind, Id> type, T term)
        {
            Term = term;
            Type = type;
            Universe = universe;
        }
    }
}