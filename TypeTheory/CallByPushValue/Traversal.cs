using System;

namespace TypeTheory.CallByPushValue
{
    public sealed class Traversal<Bind, Id, R>
    {
        private readonly Func<IClosedTermF<Bind, Id, R>, R> StepF;
        private readonly Func<Bind, ITerm<Bind, Id>> UseDeclarationF;

        public Traversal(Func<IClosedTermF<Bind, Id, R>, R> stepF, Func<Bind, ITerm<Bind, Id>> useDeclarationF)
        {
            StepF = stepF;
            UseDeclarationF = useDeclarationF;
        }

        public R Traverse(IAnnotated<Bind, Id, ITerm<Bind, Id>> annotated)
        {
            var mapping = new Mapping<Bind, Id, ITerm<Bind, Id>, R>(Traverse, UseDeclarationF);

            var unrolled = new ClosedTermF<Bind, Id, ITerm<Bind, Id>>(annotated.Environment, annotated.Expression.Fmap(term => term.Content));
            
            return StepF(mapping.Fmap(unrolled));
        }
    }
}