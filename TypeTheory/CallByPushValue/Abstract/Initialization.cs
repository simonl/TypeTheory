namespace TypeTheory.CallByPushValue
{
    public abstract class Initialization<Bind, R>
    {
        private Initialization()
        {

        }

        public abstract class Forall : Initialization<Bind, R>
        {
            private Forall()
            {

            }

            public sealed class Quantifier : Forall
            {
                public readonly Bind Parameter;
                public readonly R Body;

                public Quantifier(Bind parameter, R body)
                {
                    Parameter = parameter;
                    Body = body;
                }
            }

            public sealed class Shift : Forall
            {
                public readonly R Body;

                public Shift(R body)
                {
                    Body = body;
                }
            }
        }

        public abstract class Exists : Initialization<Bind, R>
        {
            private Exists()
            {

            }

            public sealed class Quantifier : Exists
            {
                public readonly R Left;
                public readonly R Right;

                public Quantifier(R left, R right)
                {
                    Left = left;
                    Right = right;
                }
            }

            public sealed class Shift : Exists
            {
                public readonly R Body;

                public Shift(R body)
                {
                    Body = body;
                }
            }
        }
    }
}