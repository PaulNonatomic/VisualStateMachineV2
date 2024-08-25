using System;
using System.Collections.Generic;

namespace Nonatomic.VSM2.Editor.Extensions
{
	public static class TypeExtensions
	{
		private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
		{
			{ typeof(int), "int" },
			{ typeof(short), "short" },
			{ typeof(byte), "byte" },
			{ typeof(byte[]), "byte[]" },
			{ typeof(long), "long" },
			{ typeof(double), "double" },
			{ typeof(float), "float" },
			{ typeof(decimal), "decimal" },
			{ typeof(string), "string" },
			{ typeof(bool), "bool" },
			{ typeof(object), "object" },
			{ typeof(void), "void" },
			{ typeof(char), "char" }
		};

		public static string GetSimplifiedName(this Type type)
		{
			if (TypeAliases.TryGetValue(type, out var alias))
			{
				return alias;
			}

			// If no alias found, return the original type name
			return type.Name;
		}
	}

}