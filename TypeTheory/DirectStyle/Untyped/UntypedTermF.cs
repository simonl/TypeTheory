namespace TypeTheory.DirectStyle.Untyped
{
    public abstract class UntypedTermF<Id, R>
    {
        public readonly Productions Tag;

        private UntypedTermF(Productions tag)
        {
            Tag = tag;
        }

        public sealed class Variable : UntypedTermF<Id, R>
        {
            public readonly Id Identifier;

            public Variable(Id identifier)
                : base(Productions.Variable)
            {
                Identifier = identifier;
            }
        }

        public sealed class Universe : UntypedTermF<Id, R>
        {
            public readonly IUniverse Order;

            public Universe(IUniverse order)
                : base(Productions.Universe)
            {
                Order = order;
            }
        }

        public sealed class Type : UntypedTermF<Id, R>
        {
            public readonly IAnnotated<UntypedClass<Id, R>> Class;

            public Type(IAnnotated<UntypedClass<Id, R>> @class)
                : base(Productions.Type)
            {
                Class = @class;
            }
        }

        public sealed class Constructor : UntypedTermF<Id, R>
        {
            public readonly IAnnotated<UntypedInitialization<Id, R>> Initialization;

            public Constructor(IAnnotated<UntypedInitialization<Id, R>> initialization)
                : base(Productions.Constructor)
            {
                Initialization = initialization;
            }
        }

        public sealed class Destructor : UntypedTermF<Id, R>
        {
            public readonly IAnnotated<R> Focus;
            public readonly UntypedContinuation<Id, R> Continuation;

            public Destructor(IAnnotated<R> focus, UntypedContinuation<Id, R> continuation)
                : base(Productions.Destructor)
            {
                Focus = focus;
                Continuation = continuation;
            }
        }
    }
}