using Core.Options;

namespace TypeTheory.DirectStyle.Untyped
{
    public interface IUntypedTerm<Id>
    {
        Option<IUntypedTerm<Id>> Type { get; }
        UntypedTermF<Id, IUntypedTerm<Id>> Term { get; } 
    }

    public sealed class UntypedTerm<Id> : IUntypedTerm<Id>
    {
        public Option<IUntypedTerm<Id>> Type { get; private set; }
        public UntypedTermF<Id, IUntypedTerm<Id>> Term { get; private set; }

        public UntypedTerm(Option<IUntypedTerm<Id>> type, UntypedTermF<Id, IUntypedTerm<Id>> term)
        {
            Term = term;
            Type = type;
        }
    }
}