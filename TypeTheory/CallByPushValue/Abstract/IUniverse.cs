namespace TypeTheory.CallByPushValue
{
    public interface IUniverse
    {
        uint Rank { get; }
        Polarity? Polarity { get; }
    }

    public sealed class Universe : IUniverse
    {
        public uint Rank { get; private set; }
        public Polarity? Polarity { get; private set; }

        public Universe(uint rank, Polarity? polarity)
        {
            Polarity = polarity;
            Rank = rank;
        }
    }
}