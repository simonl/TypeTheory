namespace TypeTheory.ContinuationPassing
{
    public abstract class Continuation<Id>
    {
        private Continuation()
        {
            
        }

        public sealed class Jump : Continuation<Id>
        {
            public readonly Term<Id> Argument;

            public Jump(Term<Id> argument)
            {
                Argument = argument;
            }
        }

        public sealed class Extract : Continuation<Id>
        {
            public readonly Id Left;
            public readonly Id Right;
            public readonly Term<Id> Body;

            public Extract(Id left, Id right, Term<Id> body)
            {
                Left = left;
                Right = right;
                Body = body;
            }
        }
    }
}