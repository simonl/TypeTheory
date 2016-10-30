namespace Core.Packages
{
    public sealed class Package<T> : IPackage
    {
        private readonly T Content;

        public Package(T content)
        {
            Content = content;
        }

        public R Extract<R>(IPackageReceiver<R> receiver)
        {
            return receiver.Send<T>(Content);
        }
    }
}