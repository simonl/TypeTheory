using System;
using Core.Packages;

namespace Core.Variants
{
    public sealed class Record<T> : IRecord<T>
    {
        private readonly Func<T, IPackage> GetF;

        public Record(Func<T, IPackage> getF)
        {
            GetF = getF;
        }

        public IPackage Get(T tag)
        {
            return GetF(tag);
        }
    }
}