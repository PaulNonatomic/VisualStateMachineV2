using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class FieldUtils
	{
		public static IEnumerable<FieldInfo> GetInheritedSerializedFields(Type type)
		{
			var infoFields = new List<FieldInfo>();

			while (type != null && type != typeof(UnityEngine.Object))
			{
				var publicFields = type.GetFields(BindingFlags.Instance | 
																    BindingFlags.Public | 
																    BindingFlags.DeclaredOnly);
				
				infoFields.AddRange(publicFields.Where(field 
					=> field.GetCustomAttribute<HideInInspector>() == null));
				
				var privateFields = type.GetFields(BindingFlags.Instance | 
																	 BindingFlags.NonPublic | 
																	 BindingFlags.DeclaredOnly);
				
				infoFields.AddRange(privateFields.Where(field 
					=> field.GetCustomAttribute<SerializeField>() != null));

				type = type.BaseType;
			}

			return infoFields.ToArray();
		}
	}
}