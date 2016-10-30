namespace TypeTheory.DirectStyle.Untyped
{
    public abstract class UntypedInitialization<Id, R>
    {
        private UntypedInitialization()
        {

        }

        public abstract class Forall : UntypedInitialization<Id, R>
        {
            private Forall()
            {

            }

            public sealed class Quantifier : Forall
            {
                public readonly IDeclaration<Id> Parameter;
                public readonly R Body;

                public Quantifier(IDeclaration<Id> parameter, R body)
                {
                    Parameter = parameter;
                    Body = body;
                }
            }
        }

        public abstract class Exists : UntypedInitialization<Id, R>
        {
            private Exists()
            {

            }

            public sealed class Quantifier : Exists
            {
                public readonly IBinding<Id, R> Left;
                public readonly R Right;

                public Quantifier(IBinding<Id, R> left, R right)
                {
                    Left = left;
                    Right = right;
                }
            }
        }
    }
}