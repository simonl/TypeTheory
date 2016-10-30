using Core.Packages;

namespace Core.Variants
{
    public interface IVariant<out T>
    {
        T Tag { get; }
        IPackage Content { get; }
    }
}