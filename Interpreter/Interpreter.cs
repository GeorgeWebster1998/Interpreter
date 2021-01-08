using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.FSharp.Collections;
using Interpreter.Models;

namespace Interpreter
{
    public class Interpreter
    {
        static void Main(string[] args)
        {
            int MAX_TOKENS = 50;
            LookupTable lt = new LookupTable(MAX_TOKENS); // Class to store Tokens and Symbols

            string Command = args[0].Trim(new Char[] { '[', ',', '\'', ']' }); //Decides whats returned
                                                                              
            if (Command == "rootofpoly")
            {
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                Reply reply;

                double error = 0.0;
                double seed = 0.0;
                int count = 0;
                
                var inputs = new List<double>();

                foreach(string s in text)
                {
                    if (count == 0)
                        error = Double.Parse(s);

                    else if (count == 1) 
                        seed = Double.Parse(s);

                    else 
                        inputs.Add(Double.Parse(s));

                    count++;
                }

                var fs_list = ListModule.OfSeq(inputs);

                new PositiveReply("good", null, null, NewtonRoot.CNewton(fs_list, seed, error)).PrintToConsole();

                return;
            }
            else if(Command == "parse")
            {
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                Reply reply = null;

                foreach(string s in text)
                {
                    reply = Parse(ref lt, s);

                    if (reply is ErrorReply)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(reply));
                        return;
                    }
                }

                Console.WriteLine(JsonConvert.SerializeObject(reply));

                return;

            }
            else if(Command == "expression")
            {
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                double final_result = 0.0;

                foreach (string s in text)
                {
                    Reply reply = Parse(ref lt, s);

                    if (reply is ErrorReply)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(reply));
                        return;
                    }
                    else
                    {
                        Executor executor = new Executor(ref lt);
                        double result = executor.ShuntYard();

                        final_result = result;
                        lt.ResetSymbols(MAX_TOKENS);

                    }
                }
                lt.pt.SetWidthFirst();
                new PositiveReply("good", lt.pt.width_first, lt.variables, final_result).PrintToConsole();
                return;
            }
            else
            {
                Console.WriteLine("Unknown commands");
            }
        }
        

        public static Reply Parse(ref LookupTable lt, string s)
        {
            Lexer lex = new Lexer(ref s, ref lt);
            (int, string) lexResult = lex.Process();

            if (lexResult.Item1 > 0)
            {

                Parser parser = new Parser(ref lt);
                string parseResult = parser.Parse();
                if (parseResult == "p")
                {
                    Dictionary<string, object> variables = new Dictionary<string, object>();

                    foreach (LookupTable.Symbol sym in lt.symbols)
                    {
                        if (sym.Type is LookupTable.Tokens.Variable)
                        {
                            variables.Add((string) sym.Value, null);
                        }
                    }
                    return new PositiveReply("good", null, variables, 0);
                }
                else
                {
                    return new ErrorReply("bad", "Parser Error", parseResult, s);
                }
            }
            else
            {
                return new ErrorReply("bad", "Parser Error", lexResult.Item2, s);
            }
        }

        

    }
}
