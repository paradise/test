using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
namespace Compiler
{
	public class XDomain
	{
		public AppDomain Domain { get; set; }
		public XDomain()
		{
			Domain = AppDomain.CreateDomain("builder");
			var buffer = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
			Domain.Load(buffer);
		}
		public string Run(string code)
		{
            var obj = Domain.CreateInstanceFrom(Assembly.GetExecutingAssembly().Location,
                 "Compiler.Builder", null);
			MethodInfo method = Assembly.GetExecutingAssembly().GetType("Compiler.Builder").GetMethod("Eval");
			string result = "";
			//result = method.Invoke(obj.Unwrap(), new object[] { code }).ToString();
			Builder b = new Builder();
			result = b.Eval(code);
			return result;
		}

	}
}
