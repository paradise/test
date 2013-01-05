using System.Collections.Generic;
using System.Text;

namespace CSRepl
{
    public static class ProgramBuilder
    {
        public const string UsingStatements =
            "\r\n" +
            "using System;\r\n" +
            "using System.Collections.Generic;\r\n" +
            "using System.Text;\r\n" +
            "using System.Linq;\r\n" +
            "using System.Xml;\r\n" +
            "using System.Xml.Linq;\r\n" +
            "using CSRepl;\r\n";

        private const string ClassPrefix =
            "public class Wrapper { \r\n";

        private const string InteropDeclarations =
            "  public void LoadAssembly(string name) \r\n" +
            "  { \r\n" +
            "    ExecutionContext.LoadAssembly(name); \r\n" +
            "  }\r\n" +
            "\r\n" +
            "  public bool Verbose \r\n" +
            "  {\r\n" +
            "    get { return Repl.Context.VerboseTrace; }\r\n" +
            "    set { Repl.Context.VerboseTrace = value; }\r\n" +
            "  }\r\n" +
            "\r\n" +
            "  public OutputString Exit \r\n" +
            "  {\r\n" +
            "    get { Repl.Context.Continue = false; return new OutputString(\"Bye\"); }\r\n" +
            "  }\r\n" +

            "  public OutputString exit \r\n  {\r\n    get { return Exit; }\r\n  }\r\n\r\n" +
            "  public OutputString Quit \r\n  {\r\n    get { return Exit; }\r\n  }\r\n\r\n" +
            "  public OutputString quit \r\n  {\r\n    get { return Exit; }\r\n  }\r\n\r\n" +
            "\r\n" +
            "  public OutputString Help \r\n" +
            "  {\r\n" +
            "    get { return new OutputString(@\"" +

            "  CSRepl methods:\n" +
            "    LoadAssembly(string name)\tReference the named dll\n\n" +
            "  CSRepl properties:\n" +
            "    Verbose [= true|false]\tGets or sets whether or not the output should be verbose\n" +
            "    Usings\t\t\tDisplay the current using statements\n" +
            "    Help\t\t\tDisplay help\n" +
            "    Clear\t\t\tClear the console window\n" +
            "    Quit\t\t\tExit" +

            "\"); }\r\n" +
            "  }\r\n" +

            "\r\n" +
            "  public OutputString __Program \r\n" +
            "  {\r\n" +
            "    get { return new OutputString(Repl.Program()); }\r\n" +
            "  }\r\n" +

            "\r\n" +
            "  public OutputString Usings \r\n" +
            "  {\r\n" +
            "    get { return new OutputString(ProgramBuilder.UsingStatements); }\r\n" +
            "  }\r\n" +

            "\r\n" +
            "  public OutputString Clear \r\n" +
            "  {\r\n" +
            "    get { Console.Clear(); return new OutputString(\"\"); }\r\n" +
            "  }\r\n" +

            "\r\n" +
            "  public OutputString clear \r\n" +
            "  {\r\n" +
            "    get { return Clear; }\r\n" +
            "  }\r\n\r\n";


        private const string FuncPrefix = 
            "  public object Eval() { \r\n";

        private const string ReturnStmnt = 
            "    return ";

        private const string FuncSuffix = 
            " \r\n" +
            "  }\r\n}";

        private static ExecutionContext context;

        public static string ReturnStatement(string expression)
        {
            return ReturnStmnt + expression + ";";
        }

        public static string Build(ExecutionContext executionContext, string typeDeclaration, string statement, string returnStatement, string memberDeclaration, string usingStatement)
        {
            context = executionContext;

            return UsingStatements +
                   CustomUsingStatements() +
                   usingStatement +
                   TypeDeclarations() +
                   typeDeclaration +
                   ClassPrefix +
                       MemberDeclarations() +
                       memberDeclaration +
                       InteropDeclarations +
                       FuncPrefix +
                       CallStack() +
                       statement +
                       (returnStatement ?? DefaultReturnStatement) +
                   FuncSuffix;
        }

        private static readonly string DefaultReturnStatement = ReturnStatement("\"\"");

        private static string CallStack()
        {
            return CreateInlineSectionFrom(context.CallStack);
        }

        private static string MemberDeclarations()
        {
            return CreateInlineSectionFrom(context.MemberDeclarations);
        }

        private static string TypeDeclarations()
        {
            return CreateSectionFrom(context.TypeDeclarations);
        }

        private static string CustomUsingStatements()
        {
            return CreateSectionFrom(context.UsingStatements);
        }

        private static string CreateInlineSectionFrom(IEnumerable<string> linesOfCode)
        {
            var inlineSection = new StringBuilder();
            foreach (var memberDeclaration in linesOfCode)
            {
                inlineSection.Append("    ");
                inlineSection.Append(memberDeclaration);
                inlineSection.Append("\r\n");
            }
            return inlineSection.ToString();
        }

        private static string CreateSectionFrom(IEnumerable<string> linesOfCode)
        {
            var section = new StringBuilder();
            foreach (var usingStatement in linesOfCode)
            {
                section.Append(usingStatement);
                section.Append("\r\n");
            }
            return section.ToString();
        }
    }
}