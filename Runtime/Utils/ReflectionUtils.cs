using System.Reflection;

namespace Nonatomic.VSM2.Utils
{
	public static class ReflectionUtils
	{
		public static int GetMethodIndexInType(MethodInfo methodInfo)
		{
			if (methodInfo.DeclaringType == null) return -1;

			// Get all methods declared in the type (including private and public)
			var methods = methodInfo.DeclaringType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			// Find the index of the given MethodInfo
			for (var i = 0; i < methods.Length; i++)
			{
				if (methods[i] == methodInfo)
				{
					return i;
				}
			}

			// Return -1 if the method is not found
			return -1;
		}
	}
}