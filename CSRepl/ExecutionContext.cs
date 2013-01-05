using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CSRepl
{
    public class ExecutionContext
    {
        public ExecutionContext()
        {
            Continue = true;
            MultiLineStatement = "";
        }

        public static List<string> Assemblies = new List<string>();
        public IList<string> CallStack = new List<string>();
        public IList<string> TypeDeclarations = new List<string>();
        public List<string> MemberDeclarations = new List<string>();
        public List<string> UsingStatements = new List<string>();
        public bool MultiLine { get; set; }
        public string MultiLineStatement { get; set; }
        public bool VerboseTrace { get; set; }
        public bool Continue { get; set; }

        public static void LoadAssembly(string name)
        {
            var assemblyToLoad = new FileInfo(name);
            var executingAssembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
            if (assemblyToLoad.DirectoryName != executingAssembly.DirectoryName)
            {
                if (executingAssembly.DirectoryName != null)
                {
                    if (!File.Exists(Path.Combine(executingAssembly.DirectoryName, assemblyToLoad.Name)))
                    {
                        assemblyToLoad.CopyTo(Path.Combine(executingAssembly.DirectoryName, assemblyToLoad.Name), true);
                    }
                    Assemblies.Add(assemblyToLoad.Name);
                }
            }
            else
            {
                Assemblies.Add(name);
            }
        }
    }
} 