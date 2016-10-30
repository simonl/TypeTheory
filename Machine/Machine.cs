using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public sealed class Machine
    {
        private readonly byte MemorySize;
        private readonly Instruction[] Instructions;

        public Machine(byte memorySize, Instruction[] instructions)
        {
            MemorySize = memorySize;
            Instructions = instructions;
        }

        public IState Execute(uint[] program)
        {
            var state = Initialize(program);

            while(true)
            {
                var code = state.Fetch();

                var inst = Instructions[code];

                if (inst.Perform(state))
                {
                    return state;
                }
            }
        }

        private IState Initialize(uint[] program)
        {
            var state = new State(
                memory: new Space<uint>(MemorySize), 
                registers: new uint[1 << Registers.Size]);

            for (uint i = 0; i < program.Length; i++)
            {
                state.Memory[i] = program[i];
            }

            state.Registers[Registers.HeapPointer] = (uint)program.Length;

            return state;
        }
    }
}
