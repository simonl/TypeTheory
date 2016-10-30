using Core.Packages;

namespace Core.Variants
{
    public sealed class Variant<T> : IVariant<T>
    {
        public T Tag { get; private set; }
        public IPackage Content { get; private set; }

        public Variant(T tag, IPackage content)
        {
            Content = content;
            Tag = tag;
        }
    }
}