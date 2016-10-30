using Core.Packages;

namespace Core.Variants
{
    public static class VariantUtility
    {
        public static IVariant<T> Pack<T, C>(this C content, T tag)
        {
            return new Variant<T>(tag, content.Pack());
        } 
    }
}