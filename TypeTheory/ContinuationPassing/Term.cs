namespace TypeTheory.ContinuationPassing
{
    public abstract class Term<Id>
    {
        public readonly Productions Tag;

        private Term(Productions tag)
        {
            Tag = tag;
        }

        public sealed class Variable : Term<Id>
        {
            public readonly Id Identifier;

            public Variable(Id identifier)
                : base(Productions.Variable)
            {
                Identifier = identifier;
            }
        }

        public sealed class Universe : Term<Id>
        {
            public readonly uint Rank;

            public Universe(uint rank)
                : base(Productions.Universe)
            {
                Rank = rank;
            }
        }

        public sealed class Type : Term<Id>
        {
            public readonly Structure<Id> Structure;

            public Type(Structure<Id> structure)
                : base(Productions.Type)
            {
                Structure = structure;
            }
        }

        public sealed class Constructor : Term<Id>
        {
            public readonly Initialization<Id> Initialization;
 
            public Constructor(Initialization<Id> initialization)
                : base(Productions.Constructor)
            {
                Initialization = initialization;
            }
        }

        public sealed class Destructor : Term<Id>
        {
            public readonly IQualified<Id, Term<Id>> Focus;
            public readonly Continuation<Id> Continuation; 

            public Destructor(IQualified<Id, Term<Id>> focus, Continuation<Id> continuation)
                : base(Productions.Destructor)
            {
                Focus = focus;
                Continuation = continuation;
            }
        }
    }
}
