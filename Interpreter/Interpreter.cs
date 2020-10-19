using System;
using static Lexer;
using static Parser;
using static LookupTable;

namespace Interpreter
{
    class Interpreter
    {
        static void Main()
        {
            int MAX_TOKENS = 16; //Maximum number of tokens the lexer will define
            int MAX_CHAR = 16; //Maximum characters taken from input
            LookupTable lt = new LookupTable(MAX_TOKENS); // Class to store Tokens and Symbols
            int NO_tokens; //Number of tokens
            
            while (true)
            {
                //Input
                Console.WriteLine("Try the interpreter by typing in a number ");
                char[] input = new char[MAX_CHAR];
                Console.In.Read(input);

                //Testing input
                Console.WriteLine("Input was {0}", new string(input));
                //Test over for input

                //LEXER
                Lexer lex = new Lexer(ref input,ref MAX_TOKENS, ref lt);
                Console.WriteLine("Lexer started");
                if (((NO_tokens = lex.Process())) > 1)
                    Console.WriteLine("{0} tokens found!", NO_tokens);
                else
                    Console.WriteLine("No tokens were found?!");
                
                for (int i=0; i < NO_tokens; i++)
                {
                    Console.WriteLine("Token {0} = ID,Value {1} -> {2}", i, lt.tokens[i], lt.symbols[i]);
                }

            
                
                
                Console.WriteLine("");
            }

        }
    }
}
