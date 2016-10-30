namespace TypeTheory.CallByPushValue
{
    public interface IExpression<Bind, Id, out T>
    {
        IUniverse Universe { get; }
        ITerm<Bind, Id> Type { get; }
        T Term { get; }
    }

    public sealed class Expression<Bind, Id, T> : IExpression<Bind, Id, T>
    {
        public IUniverse Universe { get; private set; }
        public ITerm<Bind, Id> Type { get; private set; }
        public T Term { get; private set; }

        public Expression(IUniverse universe, ITerm<Bind, Id> type, T term)
        {
            Term = term;
            Type = type;
            Universe = universe;
        }
    }
}