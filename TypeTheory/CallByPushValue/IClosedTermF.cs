using Core;

namespace TypeTheory.CallByPushValue
{
    public interface IClosedTermF<Bind, Id, R> : IAnnotated<Bind, Id, TermF<Bind, Id, R>>
    {

    }

    public sealed class ClosedTermF<Bind, Id, R> : IClosedTermF<Bind, Id, R>
    {
        public Sequence<IExpression<Bind, Id, Bind>> Environment { get; private set; }
        public IExpression<Bind, Id, TermF<Bind, Id, R>> Expression { get; private set; }

        public ClosedTermF(Sequence<IExpression<Bind, Id, Bind>> environment, IExpression<Bind, Id, TermF<Bind, Id, R>> expression)
        {
            Expression = expression;
            Environment = environment;
        }
    }
}