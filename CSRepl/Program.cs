using System;
using System.Diagnostics;
using System.Reflection;

namespace CSRepl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("--verbose", StringComparison.InvariantCultureIgnoreCase))
                {
                    Repl.Context.VerboseTrace = true;
                }
            }

            Trace.Listeners.Add(new ConsoleTraceListener());
            WriteWelcomeMessage();
            
            while (Repl.Context.Continue)
            {
                Console.Write("> ");
                Console.Out.Flush();
                
                var expr = Console.ReadLine();
                if (expr == null)
                {
                    break;
                }
                try
                {
                    var result = Repl.Interpret(expr);
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                catch (TargetInvocationException ex)
                {
                    WriteExceptionMessage(ex.InnerException);
                }
                catch (Exception ex)
                {
                    WriteExceptionMessage(ex);
                }
            }
        }

        private static void WriteWelcomeMessage()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine();
            Console.WriteLine("CSRepl {0}.{1} Interactive build {2}", version.Major, version.Minor, version);
            Console.WriteLine("Copyright (c) trycsharp.org. All Rights Reserved.");
            Console.WriteLine();
            Console.WriteLine("For help type Help");
            Console.WriteLine();
        }

        private static void WriteExceptionMessage(Exception ex)
        {
            Console.WriteLine("Exception of type '" + ex.GetType().Name + "' was thrown: " + ex.Message);
            
            if (Repl.Context.VerboseTrace)
            {
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.InnerException.StackTrace);
                }
            }

        }
    }
}