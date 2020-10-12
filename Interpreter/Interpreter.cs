using System;
using static Lexer;
using static Parser;

namespace Interpreter
{
    class Interpreter
    {
        static void Main(string[] args)
        {
            int MAX_TOKENS = 16;


            while (true)
            {
                Console.WriteLine("Try the interpreter by typing in a number ");
                string input = Console.ReadLine();
                Console.WriteLine("Input was {0}", input);

                Lexer lex = new Lexer(ref input, MAX_TOKENS);


            }
        }
    }
}
