using System.CodeDom.Compiler;

namespace CSRepl
{
    public class Statement : CompiledCodeSnippet
    {
        public Statement(string source, CompilerResults results) 
            : base(source, results) {}
    }
}