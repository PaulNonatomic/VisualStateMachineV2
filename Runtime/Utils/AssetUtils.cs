﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Utils
{
	public static class AssetUtils
	{
		public static List<T> FindAllScriptableObjectsOfType<T>() where T : ScriptableObject
		{
			var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			var results = new List<T>();

			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<T>(path);

				if (asset == null)
				{
					continue;
				}

				results.Add(asset);
			}

			return results.ToList();
		}
		
		public static List<Type> GetAllDerivedTypes<T>()
		{
			var derivedType = typeof(T);
			var derivedTypes = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (!type.IsSubclassOf(derivedType)) continue;
					derivedTypes.Add(type);
				}
			}

			return derivedTypes;
		}
		
		public static List<Type> FindAllDerivedTypes<T>()
		{
			var derivedType = typeof(T);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var derivedTypes = new List<Type>();

			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (derivedType.IsAssignableFrom(type) && type != derivedType)
					{
						derivedTypes.Add(type);
					}
				}
			}

			return derivedTypes;
		}
	}
}