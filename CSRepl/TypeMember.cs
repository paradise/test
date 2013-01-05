using System.CodeDom.Compiler;

namespace CSRepl
{
    public abstract class TypeMember : CompiledCodeSnippet
    {
        protected TypeMember(string source, CompilerResults results) 
            : base(source, results) {}
    }
}