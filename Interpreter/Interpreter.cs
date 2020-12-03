using System;
using InterpreterCore;

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
                Console.WriteLine("Try the interpreter by typing in basic arithmatic ");
                char[] inputRaw = new char[MAX_CHAR];
                Console.In.Read(inputRaw);

                //Testing input
                //Console.WriteLine("Input was {0}", new string(inputRaw));
                //Test over for input

                //LEXER
                Lexer lex = new Lexer(ref inputRaw, ref MAX_TOKENS, ref lt);
                Console.WriteLine("Lexer started");
                if (((NO_tokens = lex.Process())) > 1) {
                    Console.WriteLine("{0} tokens found!", NO_tokens);


                    //Parser
                    Console.WriteLine("Parser started");
                    Parser parser = new Parser(ref lt);
                    string parseResult = parser.Parse();
                    Console.WriteLine(parseResult);
                    if (parseResult == "Parsed")
                    {
                       //Executor
                        //Console.WriteLine("Executor started");
                        Executor executor = new Executor(ref lt);
                        Object result = executor.ShuntYard();

                        lt.resetSymbols(MAX_TOKENS);

                        Console.WriteLine("answer is -> {0} \n", result);

                    }
                }
                else
                    Console.WriteLine("No tokens were found?!");

        }

        }
    }
}
