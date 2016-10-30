using System;

namespace Core.Packages
{
    public static class PackageUtility
    {
        public static IPackage Pack<T>(this T content)
        {
            return new Package<T>(content);
        }

        /// <summary>
        ///     Can throw an InvalidCastException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="package"></param>
        /// <returns></returns>
        public static T Cast<T>(this IPackage package)
        {
            return package.Extract(new CastingReceiver<T>());
        }

        public static Type GetDeclaredType(this IPackage package)
        {
            return package.Extract(new TypeReceiver());
        }

        public static void Check(Type expected, Type actual)
        {
            if (expected.IsAssignableFrom(actual))
            {
                return;
            }

            throw new InvalidCastException(string.Format("Object of type {0} cannot be converted to type {1}", actual, expected));
        }
    }
}