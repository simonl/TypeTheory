namespace TypeTheory.ContinuationPassing
{
    public static class Utility
    {
        public static IQualified<Id, Term<Id>> TypeOf<Id, T>(this IQualified<Id, T> expression)
        {
            return new Qualified<Id, Term<Id>>(
                universe: new Term<Id>.Universe(expression.Universe.Rank + 1), 
                type: expression.Universe,
                term: expression.Type);
        } 
    }
}