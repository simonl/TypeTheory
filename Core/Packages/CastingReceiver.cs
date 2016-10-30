namespace Core.Packages
{
    public sealed class CastingReceiver<R> : IPackageReceiver<R>
    {
        public R Send<T>(T content)
        {
            PackageUtility.Check(expected: typeof (R), actual: typeof (T));

            return (R)(object)content;
        }
    }
}