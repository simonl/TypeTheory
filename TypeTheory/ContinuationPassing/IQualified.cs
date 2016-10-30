namespace TypeTheory.ContinuationPassing
{
    public interface IQualified<Id, out T>
    {
        Term<Id>.Universe Universe { get; }
        Term<Id> Type { get; }
        T Term { get; }
    }

    public sealed class Qualified<Id, T> : IQualified<Id, T>
    {
        public Term<Id>.Universe Universe { get; private set; }
        public Term<Id> Type { get; private set; }
        public T Term { get; private set; }

        public Qualified(Term<Id>.Universe universe, Term<Id> type, T term)
        {
            Term = term;
            Type = type;
            Universe = universe;
        }
    }
}