namespace TypeTheory.DirectStyle
{
    public abstract class Class<Bind, Id>
    {
        public readonly Classes Tag;

        private Class(Classes tag)
        {
            Tag = tag;
        }

        public sealed class Quantifier : Class<Bind, Id>
        {
            public readonly IExpression<Bind, Id, Bind> Dependency;
            public readonly Term<Bind, Id> Dependent;

            public Quantifier(IExpression<Bind, Id, Bind> dependency, Term<Bind, Id> dependent)
                : base(Classes.Quantifier)
            {
                Dependency = dependency;
                Dependent = dependent;
            }
        }

        public sealed class Fixpoint : Class<Bind, Id>
        {
            public readonly Bind Identifier;
            public readonly Term<Bind, Id> Body;

            public Fixpoint(Bind identifier, Term<Bind, Id> body)
                : base(Classes.Fixpoint)
            {
                Identifier = identifier;
                Body = body;
            }
        }
    }
}