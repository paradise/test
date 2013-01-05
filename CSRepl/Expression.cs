using System.CodeDom.Compiler;

namespace CSRepl
{
    public class Expression : Statement
    {
        public Expression(string source, CompilerResults results) : 
            base(source, results) {}
    }
}