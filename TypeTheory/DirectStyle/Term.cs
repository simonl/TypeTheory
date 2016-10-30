namespace TypeTheory.DirectStyle
{
    public abstract class Term<Bind, Id>
    {
        public readonly Productions Tag;

        private Term(Productions tag)
        {
            Tag = tag;
        }

        public sealed class Variable : Term<Bind, Id>
        {
            public readonly Id Identifier;

            public Variable(Id identifier)
                : base(Productions.Variable)
            {
                Identifier = identifier;
            }
        }

        public sealed class Universe : Term<Bind, Id>
        {
            public readonly IUniverse Order;

            public Universe(IUniverse order)
                : base(Productions.Universe)
            {
                Order = order;
            }
        }

        public sealed class Type : Term<Bind, Id>
        {
            public readonly Polarity Polarity;
            public readonly Class<Bind, Id> Class;

            public Type(Polarity polarity, Class<Bind, Id> @class)
                : base(Productions.Type)
            {
                Polarity = polarity;
                Class = @class;
            }
        }

        public sealed class Constructor : Term<Bind, Id>
        {
            public readonly Initialization<Bind, Id> Initialization;

            public Constructor(Initialization<Bind, Id> initialization)
                : base(Productions.Constructor)
            {
                Initialization = initialization;
            }
        }

        public sealed class Destructor : Term<Bind, Id>
        {
            public readonly IExpression<Bind, Id, Term<Bind, Id>> Focus;
            public readonly Continuation<Bind, Id> Continuation;

            public Destructor(IExpression<Bind, Id, Term<Bind, Id>> focus, Continuation<Bind, Id> continuation)
                : base(Productions.Destructor)
            {
                Focus = focus;
                Continuation = continuation;
            }
        }
    }
}
