namespace TypeTheory.CallByPushValue
{
    public abstract class Class<Bind, Id, R>
    {
        public enum Tags { Quantifier, Shift, }

        public readonly Tags Tag;

        private Class(Tags tag)
        {
            Tag = tag;
        }

        public sealed class Quantifier : Class<Bind, Id, R>
        {
            public readonly IExpression<Bind, Id, Bind> Dependency;
            public readonly R Dependent;

            public Quantifier(IExpression<Bind, Id, Bind> dependency, R dependent)
                : base(Tags.Quantifier)
            {
                Dependency = dependency;
                Dependent = dependent;
            }
        }

        public sealed class Shift : Class<Bind, Id, R>
        {
            public readonly R Content;

            public Shift(R content) 
                : base(Tags.Shift)
            {
                Content = content;
            }
        }
    }
}