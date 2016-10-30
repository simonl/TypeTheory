using Core.Options;

namespace TypeTheory.DirectStyle.Untyped
{
    public interface IBinding<Id, R>
    {
        R Term { get; }
        Option<Id> Identifier { get; } 
    }

    public sealed class Binding<Id, R> : IBinding<Id, R>
    {
        public R Term { get; private set; }
        public Option<Id> Identifier { get; private set; }

        public Binding(R term, Option<Id> identifier)
        {
            Identifier = identifier;
            Term = term;
        }
    }
}