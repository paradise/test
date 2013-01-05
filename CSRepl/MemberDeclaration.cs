using System.CodeDom.Compiler;

namespace CSRepl
{
    public class MemberDeclaration : TypeMember
    {
        public MemberDeclaration(string source, CompilerResults results)
            : base(source, results) {}
    }
}