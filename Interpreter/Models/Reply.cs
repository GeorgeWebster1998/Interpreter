using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Interpreter.Models.ParsedTrie;

namespace Interpreter.Models
{
    //This is the parent class of the Replies, the only sharing quality is
    //the print to console function.
    public abstract class Reply
    {
        public void PrintToConsole()
        {
            Console.WriteLine(JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            }));
        }

    }

    //This reply is used when reporting a positive return
    //status:"good"
    //abst: abstract syntax trie of the last expression/statement
    //variables: is the variables dictionary after the last expression/statement
    //output: will be the result of the last expression/statement
    public class PositiveReply : Reply
    {
        public string status;
        public ParsedTrieNode ABST;
        public Dictionary<string, object> variables;
        public double output;

        public PositiveReply(string status, ParsedTrieNode ABST, Dictionary<string, object> variables, double output)
        {
            this.status = status;
            this.ABST = ABST;
            this.variables = variables;
            this.output = output;
        }
    }

    //This reply is used when reporting an error.
    //status:"bad"
    //type: where the error occured lexer or parser
    //error: is more detail on what occured
    //location: is what expression/statement it happened on
    public class ErrorReply : Reply
    {
        public string status;
        public string type;
        public string error;
        public string location;

        public ErrorReply(string status, string type, string error, string location)
        {
            this.status = status;
            this.type = type;
            this.error = error;
            this.location = location;
        }
    }
}