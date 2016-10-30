namespace Machine
{
    public static class Operations
    {
        public static uint Fetch(this IState state)
        {
            return state.Memory[state.Registers[Registers.ProgramPointer]++];
        }

        public static void Push(this IState state, uint word)
        {
            state.Memory[--state.Registers[Registers.StackPointer]] = word;
        }

        public static uint Pop(this IState state)
        {
            return state.Memory[state.Registers[Registers.StackPointer]++];
        }

        public static uint Malloc(this IState state, uint size)
        {
            var address = state.Memory[state.Registers[Registers.HeapPointer]];

            state.Registers[Registers.HeapPointer] += size;

            return address;
        }
    }
}