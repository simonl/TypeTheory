namespace TypeTheory.CallByPushValue
{
    public interface ITerm<Bind, Id>
    {
        TermF<Bind, Id, ITerm<Bind, Id>> Content { get; }
    }

    public sealed class Term<Bind, Id> : ITerm<Bind, Id>
    {
        public TermF<Bind, Id, ITerm<Bind, Id>> Content { get; private set; }

        public Term(TermF<Bind, Id, ITerm<Bind, Id>> content)
        {
            Content = content;
        }
    }
}
