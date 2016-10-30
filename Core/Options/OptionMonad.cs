using Core.Packages;

namespace Core.Options
{
    public sealed class OptionMonad : IMonad
    {
        public IPackage Wrap<T>(T content)
        {
            var option = OptionUtility.Wrap(content);

            return option.Pack();
        }

        public IPackage Unwrap<T>(IPackage package)
        {
            var option = package.Cast<Option<Option<T>>>();

            return option.Unwrap().Pack();
        }
    }
}