namespace Core
{
    public sealed class Unit
    {
        public static readonly Unit Singleton = new Unit();

        private Unit()
        {
            
        }
    }
}