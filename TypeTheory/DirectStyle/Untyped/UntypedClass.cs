namespace TypeTheory.DirectStyle.Untyped
{
    public abstract class UntypedClass<Id, R>
    {
        private UntypedClass()
        {

        }

        public sealed class Quantifier : UntypedClass<Id, R>
        {
            public readonly IDeclaration<Id> Dependency;  
            public readonly R Dependent;

            public Quantifier(IDeclaration<Id> dependency, R dependent)
            {
                Dependency = dependency;
                Dependent = dependent;
            }
        }

        public sealed class Fixpoint : UntypedClass<Id, R>
        {
            public readonly IDeclaration<Id> Self;
            public readonly R Body;

            public Fixpoint(IDeclaration<Id> self, R body)
            {
                Self = self;
                Body = body;
            }
        }
    }
}