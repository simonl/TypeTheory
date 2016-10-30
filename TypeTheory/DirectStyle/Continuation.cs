namespace TypeTheory.DirectStyle
{
    public abstract class Continuation<Bind, Id>
    {
        private Continuation()
        {
            
        }

        public abstract class Forall : Continuation<Bind, Id>
        {
            private Forall()
            {
            
            }

            public sealed class Quantifier : Forall
            {
                public readonly Term<Bind, Id> Argument;

                public Quantifier(Term<Bind, Id> argument)
                {
                    Argument = argument;
                }
            }
        }

        public abstract class Exists : Continuation<Bind, Id>
        {
            private Exists()
            {
            
            }

            public sealed class Quantifier : Exists
            {
                public readonly Bind Left;
                public readonly Bind Right;
                public readonly Term<Bind, Id> Body;

                public Quantifier(Bind left, Bind right, Term<Bind, Id> body)
                {
                    Left = left;
                    Right = right;
                    Body = body;
                }
            }
        }
    }
}