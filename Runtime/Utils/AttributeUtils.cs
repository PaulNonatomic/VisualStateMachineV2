using System;
using System.Reflection;

namespace Nonatomic.VSM2.Utils
{
	public static class AttributeUtils
	{
		public static T GetInheritedCustomAttribute<T>(Type type) where T : Attribute
		{
			T attribute = null;

			while (type != null && attribute == null)
			{
				attribute = type.GetCustomAttribute<T>();
				type = type.BaseType;
			}

			return attribute;
		}
		
		public static bool TryGetInheritedCustomAttribute<T>(Type type, out T attribute) where T : Attribute
		{
			attribute = null;

			while (type != null && attribute == null)
			{
				attribute = type.GetCustomAttribute<T>();
				type = type.BaseType;
			}

			return attribute != null;
		}
	}
}