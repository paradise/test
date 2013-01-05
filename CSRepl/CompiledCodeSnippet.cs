using System.CodeDom.Compiler;

namespace CSRepl
{
    public abstract class CompiledCodeSnippet
    {
        public string Source { get; set; }
        public CompilerResults Results { get; set; }
        public CompilerErrorCollection Errors { get; set; }
       
        protected CompiledCodeSnippet(string source, CompilerResults results)
        {
            Source = source;
            Results = results;
            Errors = Results.Errors;
        }

        public bool HasErrors {
            get { return Errors.HasErrors; }
        } 

        public static CompiledCodeSnippet operator |(CompiledCodeSnippet a, CompiledCodeSnippet b)
        {
            return 
                a == null ? b
                    : b == null ? a
                        : b.HasErrors ? a
                            : b;
        }

        public static bool operator false(CompiledCodeSnippet a)
        {
            return false;
        }

        public static bool operator true(CompiledCodeSnippet a)
        {
            return a != null && !a.HasErrors;
        }
    }
}