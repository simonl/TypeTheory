using Core.Packages;

namespace Core.Variants
{
    public interface IRecord<in T>
    {
        IPackage Get(T field);
    }
}