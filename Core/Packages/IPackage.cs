namespace Core.Packages
{
    public interface IPackage
    {
        R Extract<R>(IPackageReceiver<R> receiver);
    }
}