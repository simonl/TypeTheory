namespace Core.Packages
{
    public interface IPackageReceiver<out R>
    {
        R Send<T>(T content);
    }
}