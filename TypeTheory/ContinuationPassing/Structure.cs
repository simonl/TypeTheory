namespace TypeTheory.ContinuationPassing
{
    public abstract class Structure<Id>
    {
        public enum Tags { Continuation, Pair, }

        public readonly Tags Tag;

        private Structure(Tags tag)
        {
            Tag = tag;
        }

        public sealed class Continuation : Structure<Id>
        {
            public readonly Term<Id> Content;

            public Continuation(Term<Id> content)
                : base(Tags.Continuation)
            {
                Content = content;
            }
        }

        public sealed class Pair : Structure<Id>
        {
            public readonly IQualified<Id, Id> Dependency;
            public readonly Term<Id> Dependent;

            public Pair(IQualified<Id, Id> dependency, Term<Id> dependent)
                : base(Tags.Pair)
            {
                Dependency = dependency;
                Dependent = dependent;
            }
        }
    }
}