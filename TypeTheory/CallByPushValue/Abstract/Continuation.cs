using System.ComponentModel;

namespace TypeTheory.CallByPushValue
{
    public abstract class Continuation<Bind, R>
    {
        private Continuation()
        {
            
        }

        public abstract class Forall : Continuation<Bind, R>
        {
            private Forall()
            {
            
            }

            public sealed class Quantifier : Forall
            {
                public readonly R Argument;

                public Quantifier(R argument)
                {
                    Argument = argument;
                }
            }

            public sealed class Shift : Forall
            {
                public readonly Bind Identifier;
                public readonly R Body;

                public Shift(Bind identifier, R body)
                {
                    Identifier = identifier;
                    Body = body;
                }
            }
        }

        public abstract class Exists : Continuation<Bind, R>
        {
            private Exists()
            {
            
            }

            public sealed class Quantifier : Exists
            {
                public readonly Bind Left;
                public readonly Bind Right;
                public readonly R Body;

                public Quantifier(Bind left, Bind right, R body)
                {
                    Left = left;
                    Right = right;
                    Body = body;
                }
            }

            public sealed class Shift : Exists
            {
                public Shift()
                {
                    
                }
            }
        }
    }
}