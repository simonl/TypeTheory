namespace TypeTheory.DirectStyle.Untyped
{
    public abstract class UntypedContinuation<Id, R>
    {
        private UntypedContinuation()
        {

        }

        public abstract class Forall : UntypedContinuation<Id, R>
        {
            private Forall()
            {

            }

            public sealed class Quantifier : Forall
            {
                public readonly IBinding<Id, R> Argument;

                public Quantifier(IBinding<Id, R> argument)
                {
                    Argument = argument;
                }
            }
        }

        public abstract class Exists : UntypedContinuation<Id, R>
        {
            private Exists()
            {

            }

            public sealed class Quantifier : Exists
            {
                public readonly IDeclaration<Id> Left;
                public readonly IDeclaration<Id> Right;
                public readonly R Body;

                public Quantifier(IDeclaration<Id> left, IDeclaration<Id> right, R body)
                {
                    Left = left;
                    Right = right;
                    Body = body;
                }
            }
        }
    }
}