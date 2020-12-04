using System;
using InterpreterCore;
using System.Linq;
using System.Collections.Generic;

namespace Interpreter
{
    class Interpreter
    {
        static void Main()
        {
            int MAX_TOKENS = 32; //Maximum number of tokens the lexer will define
            int MAX_CHAR = 32; //Maximum characters taken from input
            LookupTable lt = new LookupTable(MAX_TOKENS); // Class to store Tokens and Symbols
            int NO_tokens; //Number of tokens

            while (true)
            {
                //Input
                Console.WriteLine("Try the interpreter");
                string SIn = Console.ReadLine();
                char[] input = SIn.ToCharArray();
                
                
                //Testing input
                //Console.WriteLine("Input was {0}", new string(input));
                //Test over for input

                //LEXER
                Lexer lex = new Lexer(ref input, ref MAX_TOKENS, ref lt);
                Console.WriteLine("Lexer started");
                if (((NO_tokens = lex.Process())) > 1) {
                    Console.WriteLine("{0} tokens found!", NO_tokens);


                    //Parser
                    Console.WriteLine("Parser started");
                    Parser parser = new Parser(ref lt);
                    string parseResult = parser.Parse();
                    if (parseResult == "p")
                    {
                       //Executor
                        //Console.WriteLine("Executor started");
                        Executor executor = new Executor(ref lt);
                        Object result = executor.ShuntYard();

                        if(result is string)
                        {
                            var list = lt.variables.ToList();

                            foreach (KeyValuePair<string, LookupTable.Var> temp in list)
                            {
                                Console.WriteLine("{0} -> {1}", temp.Key, temp.Value.Value);
                            }
                            Console.WriteLine();

                        }
                        else
                        {
                            Console.WriteLine("answer is -> {0} \n", result);
                        }

                        lt.ResetSymbols(MAX_TOKENS);

                    }
                    else
                    {
                        Console.WriteLine("Parser failed with error: {0} \n", parseResult);
                    }
                }
                else
                    Console.WriteLine("No tokens were found?! \n");

        }

        }
    }
}
