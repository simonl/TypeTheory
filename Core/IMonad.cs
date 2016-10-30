using Core.Packages;

namespace Core
{
    public interface IMonad
    {
        IPackage Wrap<T>(T content);
        IPackage Unwrap<T>(IPackage package);
    }
}