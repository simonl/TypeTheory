using System;

namespace Machine
{
    public interface Instruction
    {
        bool Perform(IState state);
    }

    public sealed class Halt : Instruction
    {
        public bool Perform(IState state)
        {
            return true;
        }
    }

    public sealed class Add : Instruction
    {
        public bool Perform(IState state)
        {
            var y = state.Pop();
            var x = state.Pop();

            state.Push(x + y);

            return false;
        }
    }

    public sealed class Const : Instruction
    {
        public bool Perform(IState state)
        {
            var constant = state.Fetch();

            state.Push(constant);

            return false;
        }
    }

    public sealed class Print : Instruction
    {
        public bool Perform(IState state)
        {
            var x = state.Pop();

            Console.WriteLine(x);

            return false;
        }
    }

    public sealed class Call : Instruction
    {
        public bool Perform(IState state)
        {
            var address = state.Fetch();

            var @continue = state.Registers[Registers.ProgramPointer];

            state.Push(@continue);

            state.Registers[Registers.ProgramPointer] = address;

            return false;
        }
    }

    public sealed class Return : Instruction
    {
        public bool Perform(IState state)
        {
            var address = state.Pop();

            state.Registers[Registers.ProgramPointer] = address;

            return false;
        }
    }
}