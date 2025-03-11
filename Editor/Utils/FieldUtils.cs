using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class FieldUtils
	{
		public static IEnumerable<FieldInfo> GetInheritedSerializedFields(Type type)
		{
			var infoFields = new List<FieldInfo>();

			while (type != null && type != typeof(Object))
			{
				var publicFields = type.GetFields(BindingFlags.Instance |
												  BindingFlags.Public |
												  BindingFlags.DeclaredOnly);

				// Filter out fields with HideInInspector or NonSerialized attributes
				infoFields.AddRange(publicFields.Where(field =>
					field.GetCustomAttribute<HideInInspector>() == null &&
					field.GetCustomAttribute<NonSerializedAttribute>() == null));

				var privateFields = type.GetFields(BindingFlags.Instance |
												   BindingFlags.NonPublic |
												   BindingFlags.DeclaredOnly);

				// For private fields, they must have SerializeField attribute and not have NonSerialized
				infoFields.AddRange(privateFields.Where(field =>
					field.GetCustomAttribute<SerializeField>() != null &&
					field.GetCustomAttribute<NonSerializedAttribute>() == null));

				type = type.BaseType;
			}

			return infoFields.ToArray();
		}
	}
}