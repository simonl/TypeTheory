using Core;

namespace TypeTheory.CallByPushValue
{
    public interface IAnnotated<Bind, Id, T>
    {
        Sequence<IExpression<Bind, Id, Bind>> Environment { get; }
        IExpression<Bind, Id, T> Expression { get; }
    }

    public sealed class Annotated<Bind, Id, T> : IAnnotated<Bind, Id, T>
    {
        public Sequence<IExpression<Bind, Id, Bind>> Environment { get; private set; }
        public IExpression<Bind, Id, T> Expression { get; private set; }

        public Annotated(Sequence<IExpression<Bind, Id, Bind>> environment, IExpression<Bind, Id, T> expression)
        {
            Expression = expression;
            Environment = environment;
        }
    }
}