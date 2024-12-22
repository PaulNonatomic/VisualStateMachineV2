using System.IO;

namespace Nonatomic.VSM2.Editor.Migration
{
#if UNITY_EDITOR
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Nonatomic.VSM2.StateGraph;
	using UnityEditor;
	using UnityEngine;

	public static class MigrationUtils
	{
		public static List<Type> FindAllDerivedStates()
		{
			var baseType = typeof(State); // Nonatomic.VSM2.StateGraph.State
			var derivedTypes = new List<Type>();
		
			// Get all assemblies in the current AppDomain.
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		
			foreach (var assembly in assemblies)
			{
				// You may want to skip certain assemblies (e.g., system assemblies)
				if (assembly.FullName.StartsWith("System") ||
					assembly.FullName.StartsWith("Microsoft") ||
					assembly.FullName.StartsWith("netstandard") ||
					assembly.FullName.StartsWith("Unity"))
				{
					continue;
				}

				// Attempt to retrieve all types from the assembly.
				Type[] types;
				try
				{
					types = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException e)
				{
					// In case some types cannot be loaded, fallback to what we can load
					types = e.Types.Where(t => t != null).ToArray();
				}

				// Check each type to see if it inherits from Nonatomic.VSM2.StateGraph.State
				foreach (var type in types)
				{
					if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType))
					{
						derivedTypes.Add(type);
					}
				}
			}

			return derivedTypes;
		}
		
		public static string GetPathForType(System.Type type)
		{
			// Search for all MonoScripts in the project
			var guids = AssetDatabase.FindAssets("t:MonoScript");
		
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

				// Skip if we can’t load or type doesn't match
				if (monoScript == null) continue;
				if (monoScript.GetClass() != type) continue;

				// Convert relative asset path to absolute system path
				var fullPath = Application.dataPath.Replace("Assets", "") + path;
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			Debug.LogWarning($"Could not find a .cs file for type: {type.FullName}");
			return null;
		}
	}
#endif

}