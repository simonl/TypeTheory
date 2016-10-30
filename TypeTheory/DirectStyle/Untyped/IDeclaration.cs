using Core.Options;

namespace TypeTheory.DirectStyle.Untyped
{
    public interface IDeclaration<Id>
    {
        Option<IUntypedTerm<Id>> Type { get; }
        Id Identifier { get; }
    }

    public sealed class Declaration<Id> : IDeclaration<Id>
    {
        public Option<IUntypedTerm<Id>> Type { get; private set; }
        public Id Identifier { get; private set; }

        public Declaration(Option<IUntypedTerm<Id>> type, Id identifier)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}