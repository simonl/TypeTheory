using System;
using Core.Packages;
using Core.Variants;

namespace Core.Options
{
    public static class OptionUtility
    {
        public static IVariant<Option<T>.Tags> ToVariant<T>(this Option<T> option)
        {
            return new Variant<Option<T>.Tags>(option.Tag, option.GetContent());
        } 

        public static IPackage GetContent<T>(this Option<T> option)
        {
            switch (option.Tag)
            {
                case Option<T>.Tags.None:
                {
                    var none = (Option<T>.None) option;

                    return none.Content.Pack();
                }
                case Option<T>.Tags.Some:
                {
                    var some = (Option<T>.Some) option;

                    return some.Content.Pack();
                }
                default:
                {
                    throw new InvalidProgramException("Should never happen.");
                }
            }
        }

        public static Option<T> Build<T>(IVariant<Option<T>.Tags> variant)
        {
            switch (variant.Tag)
            {
                case Option<T>.Tags.None:
                {
                    var content = variant.Content.Cast<Unit>();

                    return new Option<T>.None(content);
                }
                case Option<T>.Tags.Some:
                {
                    var content = variant.Content.Cast<T>();

                    return new Option<T>.Some(content);
                }
                default:
                {
                    throw new InvalidProgramException("Should never happen.");
                }
            }
        }

        public static Option<T> Merge<T>(IVariant<uint> variant)
        {
            Func<Type, Type> optionFunctor = (Type type) => typeof (Option<>).MakeGenericType(type);

            var merged = MergeRefl<T>(optionFunctor, new OptionMonad(), variant);

            return merged.Cast<Option<T>>();
        }

        public static IPackage MergeRefl<T>(Func<Type, Type> functor, IMonad monad, IVariant<uint> variant)
        {
            switch (variant.Tag)
            {
                case 0U:
                    {
                        var content = variant.Content.Cast<T>();

                        return monad.Wrap<T>(content);
                    }
                case 1U:
                    {
                        return variant.Content;
                    }
                default:
                    {
                        variant = new Variant<uint>(variant.Tag - 1, variant.Content);

                        var generic = functor(typeof(T));

                        var merged = (IPackage) typeof(OptionUtility)
                            .GetMethod("MergeRefl")
                            .MakeGenericMethod(generic)
                            .Invoke(null, new object[] { functor, monad, variant });

                        return monad.Unwrap<T>(merged);
                    }
            }
        } 

        public static Option<T> Wrap<T>(T content)
        {
            return new Option<T>.Some(content);
        }

        public static Option<A> Unwrap<A>(this Option<Option<A>> option)
        {
            switch (option.Tag)
            {
                case Option<Option<A>>.Tags.None:
                    var none = (Option<Option<A>>.None)option;

                    return new Option<A>.None(none.Content);
                case Option<Option<A>>.Tags.Some:
                    var some = (Option<Option<A>>.Some)option;

                    return some.Content;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}