using System;

namespace GridWorld
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Grid grid = new Grid("world3.txt");
            Console.WriteLine(grid.PrintValueFunction(ValueIteration.EvaluateUniformRandomPolicy(grid)));
            Console.WriteLine("Hello World!");
        }
    }
}
