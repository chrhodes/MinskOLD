using System;

namespace mc
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTE(crhodes)
            // Establish Initial REPL (Read-Evaluate-Print-Loop)

            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                if (line == "1 + 2 * 3")
                {
                    Console.WriteLine("7");
                }
                else
                {
                    Console.WriteLine("Error: Invalid expression!");
                }
            }
        }
    }
}
