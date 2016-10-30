namespace Machine
{
    public interface IState
    {
        ISpace<uint> Memory { get; }
        uint[] Registers { get; }
    }

    public sealed class State : IState
    {
        public ISpace<uint> Memory { get; private set; }
        public uint[] Registers { get; private set; }

        public State(ISpace<uint> memory, uint[] registers)
        {
            Memory = memory;
            Registers = registers;
        }
    }
}