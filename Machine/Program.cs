using System;

namespace Machine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var machine = new Machine(8, new Instruction[]
            {
                new Halt(), 
                new Const(), 
                new Add(), 
                new Print(),
            });

            var state = machine.Execute(new uint[]
            {
                1, 2,
                1, 3,
                2,
                3,
                0
            });

            Console.ReadLine();
        }
    }
}