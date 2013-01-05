using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;

namespace CSRepl
{
    public static class Repl {
        private static readonly CodeDomProvider Compiler = CodeDomProvider.CreateProvider("C#");
        public static ExecutionContext Context;

        static Repl()
        {
            Context = new ExecutionContext();
        }

        public static void Reset()
        {
            Context = new ExecutionContext();
        }

        /// <summary>
        /// Interprets the provied source code and executes it in the current execution context
        /// </summary>
        /// <param name="sourceCode">a piece of C# code</param>
        /// <returns>the result of the execution, or null if the execution didn't yield a result</returns>
        public static string Interpret(string sourceCode)
        {
            return sourceCode
                .CompileCodeSnippet()
                .Invoke();
        }

        private static CompiledCodeSnippet CompileCodeSnippet(this string code)
        {
            if (Context.MultiLine)
            {
                Context.MultiLineStatement += code;
                code = Context.MultiLineStatement;
            }
            
            return
                code.Statement()
                || code.TypeMember();
        }

        private static CompiledCodeSnippet Statement(this string code)
        {
            return
                code.ExpressionStatement()
                || code.UsingStatement()
                || code.GenericStatement();
        }

        /// <summary>
        /// A using statement starts with "using "
        /// </summary>
        /// <param name="code">a line of C# code</param>
        /// <returns>a <see cref="CompiledCodeSnippet"/> if <paramref name="code"/> is a using statement,
        /// otherwise null</returns>
        private static CompiledCodeSnippet UsingStatement(this string code)
        {
            CompiledCodeSnippet codeSnippet = null;
            if (code.TrimStart().StartsWith("using "))
            {
                var statementCode = code.TrimEnd();
                if (!statementCode.EndsWith(";"))
                {
                    statementCode += ";";
                }
                var program = Program(usingStatement: statementCode);
                var statement = new Statement(code, CompileFromSource(program));
                if (!statement.HasErrors)
                {
                    Context.UsingStatements.Add(statementCode);
                    codeSnippet = statement;
                }
            }
            return codeSnippet;
        }

        private static CompiledCodeSnippet GenericStatement(this string code)
        {
            CompiledCodeSnippet codeSnippet = null;
            var program = Program(statement: code + ";");

            var statement = new Statement(code, CompileFromSource(program));
            if (!statement.HasErrors)
            {
                Context.CallStack.Add(code + ";");
                codeSnippet = statement;
            }
            else if (!Context.MultiLine &&
                        (statement.Errors[0].ErrorNumber == CompilerErrors.CS1513 ||
                        statement.Errors[0].ErrorNumber == CompilerErrors.CS1528))
            {
                Context.MultiLine = true;
                Context.MultiLineStatement += code;
            }

            return codeSnippet;
        }

        private static CompiledCodeSnippet ExpressionStatement(this string expr)
        {
            var expression = new Expression(expr, 
                                    Program(
                                        returnStatement: ProgramBuilder.ReturnStatement(expr)
                                        )
                                        .CompileFromSource());
            if (!expression.HasErrors)
            {
                if (!expr.Trim().Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    var name = "__" + Guid.NewGuid().ToString().Replace("-", "");
                    Context.CallStack.Add("var " + name + " = " + expr + ";");
                }
            }
            
            return expression;
        }

        public static string Program(string typeDeclaration = null, string statement = null, string returnStatement = null, string memberDeclaration = null, string usingStatement = null)
        {
            return ProgramBuilder.Build(Context, typeDeclaration, statement, returnStatement, memberDeclaration, usingStatement);
        }

        private static CompiledCodeSnippet TypeMember(this string source)
        {
            return
                source.TypeDeclaration()
                || source.MemberDeclaration()
                || source.FieldDeclaration();
        }

        private static CompiledCodeSnippet MemberDeclaration(this string code)
        {
            var memberDeclaration = new MemberDeclaration(
                code, 
                CompileFromSource(
                    Program(
                        memberDeclaration: code)));
            if (!memberDeclaration.HasErrors)
                Context.MemberDeclarations.Add(code);
            return memberDeclaration;
        }

        private static CompiledCodeSnippet TypeDeclaration(this string source)
        {
            var program = Program(typeDeclaration: source);

            var typeDeclaration = new TypeDeclaration(source, CompileFromSource(program));
            if (!typeDeclaration.HasErrors)
                Context.TypeDeclarations.Add(source);
            return typeDeclaration;
        }

        private static CompiledCodeSnippet FieldDeclaration(this string code)
        {
            var source = code + ";";
            var fieldDeclaration = new MemberDeclaration(
                code,
                CompileFromSource(
                    Program(
                        memberDeclaration: source)));
            if (!fieldDeclaration.HasErrors)
                Context.MemberDeclarations.Add(source);
            return fieldDeclaration;
        }

        private static string Invoke(this CompiledCodeSnippet compiledCode)
        {
            if (Context.MultiLine && !compiledCode.HasErrors)
            {
                Context.MultiLine = false;
                Context.MultiLineStatement = "";
            }

            if (!Context.MultiLine && compiledCode.HasErrors)
            {
                TraceErrorMessage(compiledCode);
            }

            if (!Context.MultiLine && !compiledCode.HasErrors && (compiledCode is Expression || compiledCode is Statement))
            {
                Context.MultiLine = false;
                Context.MultiLineStatement = "";				
                var result = InvokeCompiledResult(compiledCode);

                if (compiledCode is Expression)
                    return result.FormatOutput();
            }

            return null;
        }

        private static void TraceErrorMessage(CompiledCodeSnippet compiledCode)
        {
            Trace.TraceError(compiledCode.Errors[0].ErrorText);
            
            if (Context.VerboseTrace)
            {
                Trace.TraceError(compiledCode.Errors[0].ErrorNumber);
            }
        }
		static void p()
		{}
		static List<Assembly> tempAss = new List<Assembly>();
		private static object InvokeCompiledResult(CompiledCodeSnippet code)
        {
			var results = code.Results;
            Assembly assm = results.CompiledAssembly;
			tempAss.Add(assm);
			//assm.PermissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.NoAccess, "c:\\"));			
            Type target = assm.GetType("Wrapper");
            object obj = Activator.CreateInstance(target, null);			
            MethodInfo method = target.GetMethod("Eval");
			return method.Invoke(obj, null);			
			//var d = AppDomain.CreateDomain("sfgfdgfdg");
			//System.Runtime.InteropServices.RegistrationServices reg = new System.Runtime.InteropServices.RegistrationServices();
			//reg.RegisterAssembly(assm, System.Runtime.InteropServices.AssemblyRegistrationFlags.None);
			//d.SetData("ass", assm);
			//d.AssemblyResolve += new ResolveEventHandler(d_AssemblyResolve);

			//d.DefineDynamicAssembly(assm.GetName(),System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect);
			//d.Load(assm.FullName);
			///object obj1= d.CreateInstance(assm.FullName, "Wrapper");
			//object obj1 = d.CreateInstanceAndUnwrap(, "Wrapper");
			//MethodInfo method1 = target.GetMethod("Eval");

			//XDomain domain = new XDomain();
			//domain.BuildCode("2+2");
			//return domain.Run("2+2");
			//return "";
			//return method1.Invoke(obj, null);


			//CrossAppDomainDelegate del = new CrossAppDomainDelegate(delegate() { method.Invoke(obj,null); });
			//d.DoCallBack(del);
			//return 1;
        }

		static Assembly d_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return AppDomain.CurrentDomain.GetData("ass") as Assembly;
			//var str = AppDomain.CurrentDomain.FriendlyName;
			//if (args.Name == tempAss[0].FullName)
			//    return tempAss[0];
			//return null;
		}

        private static CompilerResults CompileFromSource(this string source)
        {
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };			
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location); // "CSRepl.exe"
            options.ReferencedAssemblies.Add("System.Xml.dll");
            options.ReferencedAssemblies.Add("System.Xml.Linq.dll");			
            foreach (var assembly in ExecutionContext.Assemblies)
            {
                options.ReferencedAssemblies.Add(assembly);
            }						
            return Compiler.CompileAssemblyFromSource(options, source);
        }
    }
}