using System;
using System.Security.Cryptography;

namespace TypeTheory.ContinuationPassing
{
    public sealed class Display<Id>
    {
        private readonly Func<Id, string> IdToString;

        public Display(Func<Id, string> idToString)
        {
            IdToString = idToString;
        }

        public string ToString(IQualified<Id, Term<Id>> expression)
        {
            return ToString(expression.Term);
        }

        public string ToString(Term<Id> term)
        {
            switch (term.Tag)
            {
                case Productions.Variable:
                {
                    var variable = (Term<Id>.Variable)term;

                    return "Variable(" + IdToString(variable.Identifier) + ")";
                }
                case Productions.Universe:
                {
                    var universe = (Term<Id>.Universe) term;

                    return "Universe(" + universe.Rank + ")";
                }
                case Productions.Type:
                {
                    var type = (Term<Id>.Type) term;

                    switch (type.Structure.Tag)
                    {
                        case Structure<Id>.Tags.Continuation:
                            var continuation = (Structure<Id>.Continuation)type.Structure;

                            return "Not(" + ToString(continuation.Content) + ")";
                        case Structure<Id>.Tags.Pair:
                            var pair = (Structure<Id>.Pair) type.Structure;

                            return "Exists(" + IdToString(pair.Dependency.Term) + ": " + ToString(pair.Dependency.TypeOf()) + ", " + ToString(pair.Dependent) + ")";
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }
                }
                case Productions.Constructor:
                    var constructor = (Term<Id>.Constructor)term;

                    if (constructor.Initialization is Initialization<Id>.Lambda)
                    {
                        var lambda = (Initialization<Id>.Lambda) constructor.Initialization;

                        return "Lambda(" + IdToString(lambda.Parameter) + ", " + ToString(lambda.Body) + ")";
                    }
                    else
                    {
                        var pair = (Initialization<Id>.Pair) constructor.Initialization;

                        return "Pair(" + ToString(pair.Left) + ", " + ToString(pair.Right) + ")";
                    }

                case Productions.Destructor:
                    var destructor = (Term<Id>.Destructor)term;

                    if (destructor.Continuation is Continuation<Id>.Jump)
                    {
                        var jump = (Continuation<Id>.Jump)destructor.Continuation;

                        return "Jump(" + ToString(destructor.Focus) + ", " + ToString(jump.Argument) + ")";
                    }
                    else
                    {
                        var extract = (Continuation<Id>.Extract)destructor.Continuation;

                        return "Extract(" + ToString(destructor.Focus) + ", " + IdToString(extract.Left) + ", " + IdToString(extract.Right) + ", " + ToString(extract.Body) + ")";
                    }

                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}