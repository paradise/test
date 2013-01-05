using System.CodeDom.Compiler;

namespace CSRepl
{
    public class TypeDeclaration : TypeMember
    {
        public TypeDeclaration(string source, CompilerResults compilerResults) 
            : base(source, compilerResults) {}
    }
}