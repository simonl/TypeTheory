namespace TypeTheory.DirectStyle
{
    public interface IUniverse
    {
        uint Rank { get; }
    }

    public sealed class Universe : IUniverse
    {
        public uint Rank { get; private set; }

        public Universe(uint rank)
        {
            Rank = rank;
        }
    }
}