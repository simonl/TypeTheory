namespace TypeTheory.DirectStyle
{
    public abstract class Initialization<Bind, Id>
    {
        private Initialization()
        {

        }

        public abstract class Forall : Initialization<Bind, Id>
        {
            private Forall()
            {

            }

            public sealed class Quantifier : Forall
            {
                public readonly Bind Parameter;
                public readonly Term<Bind, Id> Body;

                public Quantifier(Bind parameter, Term<Bind, Id> body)
                {
                    Parameter = parameter;
                    Body = body;
                }
            }
        }

        public abstract class Exists : Initialization<Bind, Id>
        {
            private Exists()
            {

            }

            public sealed class Quantifier : Exists
            {
                public readonly Term<Bind, Id> Left;
                public readonly Term<Bind, Id> Right;

                public Quantifier(Term<Bind, Id> left, Term<Bind, Id> right)
                {
                    Left = left;
                    Right = right;
                }
            }
        }
    }
}