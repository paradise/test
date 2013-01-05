using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
namespace Compiler
{
	[Serializable]
	public class Builder : MarshalByRefObject
	{
		public string GetFullSourceCode(string code)
		{
			return @" 
			using System;
			using System.IO;
			using System.Collections.Generic;
			using System.Linq;
			using System.Text;
			namespace Compiler 
			{ 
				public class Executer
				{	
					System.IO.MemoryStream _out;
					StreamWriter sw;
		
					public Executer()
					{
						_out = new System.IO.MemoryStream();	
						sw = new System.IO.StreamWriter(_out);
					}
					public void Init()
					{
						Console.SetOut(sw);			
					}		
					public Stream GetOut()
					{
						sw.Flush();
						_out.Position = 0;
						return _out;
					}
					public void Eval()
					{	try
						{" + code + @"		
						}
						catch(Exception e)
						{
							sw.WriteLine(e.Message);
						}
					}
		
				} 
			} ";
		}

		public bool CheckUsings(string code)
		{
			Regex regex = new Regex(@"(?(?=System\.[aA-zZ0-9_\.]+)((System\.(?!IO|Linq|Text)[aA-zZ0-9_\.]+)|(System\.IO\.(IsolatedStorage|Management|Compression|MemoryMappedFiles|Pipes|Ports)))|(Microsoft\.[aA-zZ0-9_\.]+))");
			return regex.IsMatch(code);
		}

		public bool CheckClasses(string code)
		{
			//System.IO.UnmanagedMemoryAccessor
			return false;
		}		

		public string Eval(string code)
		{							
			string result = "";
			if (CheckClasses(code)|| CheckUsings(code))
				return "Using bad classes or namespaces";			
			if (!string.IsNullOrEmpty(result))
				return result;
			CodeDomProvider compiler = CodeDomProvider.CreateProvider("C#");
			var options = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
			};
			options.ReferencedAssemblies.Add("System.Core.dll");
			//options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
			options.ReferencedAssemblies.Add("System.Xml.dll");
			options.ReferencedAssemblies.Add("System.Xml.Linq.dll");

			string source = GetFullSourceCode(code);
			
			CompilerResults results = compiler.CompileAssemblyFromSource(options, source);
			foreach (CompilerError err in results.Errors)
				result += string.Format("ERROR {0}", err.ErrorText) + Environment.NewLine;
			if (results.Errors.Count == 0)
			{
				Assembly assm = results.CompiledAssembly;
				Type target = assm.GetType("Compiler.Executer");
				object obj = Activator.CreateInstance(target, null);
				MethodInfo init = target.GetMethod("Init");
				init.Invoke(obj, null);
				MethodInfo eval = target.GetMethod("Eval");
				eval.Invoke(obj, null);
				MethodInfo outMethod = target.GetMethod("GetOut");
				var _out = outMethod.Invoke(obj, null) as System.IO.MemoryStream;
				result += new System.IO.StreamReader(_out).ReadToEnd();
			}
			return result;
            
		}

	}
}
