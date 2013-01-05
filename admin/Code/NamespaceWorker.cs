using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace admin.Code
{
	public class NamespaceWorker
	{
		private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
		}

		public static List<string> GetFullNamespaceInfo(string ns)
		{
			return GetTypesInNamespace(Assembly.Load("mscorlib.dll", null), ns).Select(o => o.FullName).ToList();
		}
	}
}