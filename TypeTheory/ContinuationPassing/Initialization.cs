namespace TypeTheory.ContinuationPassing
{
    public abstract class Initialization<Id>
    {
        private Initialization()
        {

        }

        public sealed class Lambda : Initialization<Id>
        {
            public readonly Id Parameter;
            public readonly Term<Id> Body;

            public Lambda(Id parameter, Term<Id> body)
            {
                Parameter = parameter;
                Body = body;
            }
        }

        public sealed class Pair : Initialization<Id>
        {
            public readonly Term<Id> Left;
            public readonly Term<Id> Right;

            public Pair(Term<Id> left, Term<Id> right)
            {
                Left = left;
                Right = right;
            }
        }
    }
}