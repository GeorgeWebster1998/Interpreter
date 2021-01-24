using Interpreter.Models;
using Microsoft.FSharp.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Interpreter
{
    public class Controller
    {
        public static void Main(string[] args)
        {
            Config config = new Config();
            LookupTable lt = new LookupTable(config.MAX_TOKENS); // Class to store Tokens and Symbols
            string Command = "";

            try
            {
                //this is the command string which chooses which operation the interpreter will do
                Command = args[0].Trim(new Char[] { '[', ',', '\'', ']' });
            }
            catch (Exception e)
            {
                new ErrorReply("Controller error", "No arguements given", Command).PrintToConsole();
                return;
            }

            //This sets it down the newton raphson root root.
            if (Command == "rootofpoly")
            {
                //This deserializes the list of doubles used for the method
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                //These are preset from the config file and are used for the f# functions
                double error = config.ErrorMargin;
                double seed = config.Seed;

                //This loops over each double and adds it to a list
                //which is then converted to a f# list
                var inputs = new List<double>();
                foreach (string s in text)
                {
                    inputs.Add(Double.Parse(s));
                }
                var fs_list = ListModule.OfSeq(inputs);

                //This calls and replys back the result from the f# method
                new PositiveReply(null, null, NewtonRoot.CNewton(fs_list, seed, error)).PrintToConsole();

                return;
            }
            //this fork just parses the expression/statement
            else if (Command == "parse")
            {
                //deserializes the list of expressions/statements
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                //inits a reply
                Reply reply = null;

                //This loops over every expression/statement
                foreach (string s in text)
                {
                    //This sets the reply from the parse function
                    reply = Parse(ref lt, s, true);

                    //If an error has occured te interpreter returns it before parsing all loops
                    if (reply is ErrorReply)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(reply));
                        return;
                    }

                    Executor exe = new Executor(ref lt, true);

                    try
                    {
                        exe.ShuntYard();
                        lt.operations = exe.operations;
                    }
                    catch (Exception e)
                    {
                        new ErrorReply("Executor error", e.Message, s).PrintToConsole();
                        return;
                    }

                }

                //if no error has occured it returns the reply of the last statement/error 

                new PositiveReply(lt.operations, lt.variables, 0).PrintToConsole();

                return;

            }
            //This fork both parses and executes the list of expressions/statements
            else if (Command == "expression")
            {
                dynamic text = JsonConvert.DeserializeObject(args[1]);

                double final_result = 0.0;

                foreach (string s in text)
                {
                    Reply reply = Parse(ref lt, s, false);

                    //if an error is found at any point it returns early with an error
                    if (reply is ErrorReply)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(reply));
                        return;
                    }
                    //if not it runs the executor
                    else
                    {
                        //Init the executor and get the result of parsed tokens
                        //then resets the symbol table for the next expression/statement
                        double result = 0.0;
                        Executor exe = new Executor(ref lt, false);

                        try
                        {
                            result = exe.ShuntYard();
                        }
                        catch (Exception e)
                        {
                            new ErrorReply("Executor error", e.Message, s).PrintToConsole();
                            return;
                        }

                        lt.InitSymbols(config.MAX_TOKENS);
                        lt.operations = exe.operations;

                        //This is used for output
                        final_result = result;
                    }
                }
                //This sets the abst for output and then sends the reply
                //lt.pt.SetAST();
                new PositiveReply(lt.operations, lt.variables, final_result).PrintToConsole();
                return;
            }
            else if (Command == "env")
            {
                while (true)
                {
                    string input = Console.ReadLine();
                    double final_result = 0.0;

                    if (input == "EXIT")
                    {
                        return;
                    }

                    Reply reply = Parse(ref lt, input, false);

                    //if an error is found at any point it returns early with an error
                    if (reply is ErrorReply)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(reply));
                    }
                    //if not it runs the executor
                    else
                    {
                        //Init the executor and get the result of parsed tokens
                        //then resets the symbol table for the next expression/statement
                        double result = 0.0;
                        Executor exe = new Executor(ref lt, false);

                        try
                        {
                            result = exe.ShuntYard();
                        }
                        catch (Exception e)
                        {
                            new ErrorReply("Executor error", e.Message, input).PrintToConsole();
                        }

                        lt.InitSymbols(config.MAX_TOKENS);
                        lt.operations = exe.operations;

                        //This is used for output
                        final_result = result;
                        new PositiveReply(lt.operations, lt.variables, final_result).PrintToConsole();
                    }
                }
            }
            //if the command variable is not recognised it will throw this error
            else
            {
                new ErrorReply("Interpreter error", "Unknown command", Command).PrintToConsole();
            }
        }

        /// <summary>
        /// This function is the parser but modular so both forks can use it
        /// </summary>
        /// <param name="lt"> ref LookupTable</param>
        /// <param name="s"> string s</param>
        /// <param name="isFromParseFunc"> bool for checking which function the call came from</param>
        /// <returns></returns>
        public static Reply Parse(ref LookupTable lt, string s, bool isFromParseFunc)
        {
            //This inits the lexer and processes it
            Lexicon lex = new Lexicon(ref s, ref lt);
            (int, string) lexResult = lex.Process();

            //If the lexer recognises more than one token it will parse the tokens
            if (lexResult.Item1 > 0)
            {
                //Parsing begins
                Parser parser = new Parser(ref lt, isFromParseFunc);
                string parseResult = parser.Parse();

                //If the parser correctly parses
                if (parseResult == "p")
                {
                    //Creates a temporary variables dictionary
                    //This is used to see if a stement/expression is BNF correct and
                    //what variables it needs to execute.
                    if (isFromParseFunc)
                    {

                        return new PositiveReply(null, lt.variables, 0);
                    }
                    return new PositiveReply(null, null, 0);
                }
                //If the parser encounters an error
                else
                {
                    return new ErrorReply("Parser Error", parseResult, s);
                }
            }
            //If the lexer doesn't recognise any tokens it will throw this error
            else
            {
                return new ErrorReply("Lexicon Error", lexResult.Item2, s);
            }
        }
    }
}
