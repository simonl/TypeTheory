using System;

namespace Core.Packages
{
    public sealed class TypeReceiver : IPackageReceiver<Type>
    {
        public Type Send<T>(T content)
        {
            return typeof (T);
        }
    }
}