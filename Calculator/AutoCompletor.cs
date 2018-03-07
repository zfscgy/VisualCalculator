using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class AutoCompletor
    {
        Dictionary<string, string>  CompletionDict = new Dictionary<string, string>
        {
            {"c","os(" },
            {"s","in(" },
            {"p","i" },
            {"i","nt(" },
        };
        public AutoCompletor() { }
        public string Completion(string tail)
        {
            try
            {
                return CompletionDict[tail];
            }
            catch(Exception e)
            {
                return "";
            }
        }
    }
}
