using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Interpreter.Models
{
    public abstract class Reply
    {
        public void PrintToConsole()
        {
            Console.WriteLine(JsonConvert.SerializeObject(this));
        }

    }

    public class PositiveReply : Reply
    {

        public string status;
        public List<List<string>> AST;
        public Dictionary<string, object> variables;
        public double output;

        public PositiveReply(string status, List<List<string>> AST, Dictionary<string, object> variables, double output)
        {
            this.status = status;
            this.AST = AST;
            this.variables = variables;
            this.output = output;
        }
    }

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