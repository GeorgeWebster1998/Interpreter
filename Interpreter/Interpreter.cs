using System;
using InterpreterCore;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.FSharp.Collections;


namespace Interpreter
{
    public class Interpreter
    {
        static void Main(string[] args)
        {
            int MAX_TOKENS = 50;
            LookupTable lt = new LookupTable(MAX_TOKENS); // Class to store Tokens and Symbols

            string Command = args[0].Trim(new Char[] { '[', ',', '\'', ']' }); //Decides whats returned
                                                                               //Console.WriteLine(Command);

            if (Command == "rootofpoly")
            {
                double error = Double.Parse(args[1].Trim(new Char[] { '[', ',', '\'', ']' }));
                double seed = Double.Parse(args[2].Trim(new Char[] { '[', ',', '\'', ']' }));
                var inputs = new List<double>();

                for (int i = 3; i < args.Length; i++)
                {
                    inputs.Add(Double.Parse(args[i].Trim(new Char[] { '[', ',', '\'', ']' })));
                }

                var fs_list = ListModule.OfSeq(inputs);
                double result = NewtonRoot.CNewton(fs_list, seed, error);

                Console.WriteLine(result);
                Environment.Exit(1);
            }



            int InputCount = args.Length;
            string[] Inputs = new string[InputCount];  

            (int,string) Errors = (0,"");

            for (int i = 1; i < InputCount; i++)
            {
                Inputs[i] = args[i].Trim(new Char[] { '[', ',', '\'', ']' });
                char[] line = Inputs[i].ToCharArray();

                Lexer lex = new Lexer(ref line, ref MAX_TOKENS, ref lt);
                (int, string) lexResult = lex.Process();

                if (lexResult.Item1 > 1)
                {
                    Parser parser = new Parser(ref lt);
                    string parseResult = parser.Parse();
                    if (parseResult == "p")
                    {
                        Executor executor = new Executor(ref lt);
                        Object result = executor.ShuntYard();

                        if (!(result is string))
                        {
                            lt.pt.final_result = (double) result;
                        }

                        lt.ResetSymbols(MAX_TOKENS);

                    }
                    else
                    {
                        Errors.Item1 += 1;
                        Errors.Item2 = String.Format("{0}Parser failed on arguement \"{1}\" with error: {2}. ", Errors.Item2, Inputs[i], parseResult);
                    }
                }
                else
                {
                    Errors.Item1 += 1;
                    Errors.Item2 = String.Format("{0}Lexer failed on arguement \"{1}\" with error: {2}. ", Errors.Item2, Inputs[i], lexResult.Item2);
                }
           }
            
            if (Errors.Item1 > 0)
            {
                Console.WriteLine(Errors.Item2);
            }
            else if (Command.Equals("expression"))
            {
                lt.pt.SetWidthFirst();
                Console.WriteLine(JsonConvert.SerializeObject(lt.pt));
            }
            else if (Command.Equals("statement"))
            {
                Console.WriteLine(JsonConvert.SerializeObject(lt.variables));
            }
        }
    }
}
