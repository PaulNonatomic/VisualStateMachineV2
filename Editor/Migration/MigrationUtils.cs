using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public static class MigrationUtils
{
	public static string GetPathForType(Type type)
	{
		var guids = AssetDatabase.FindAssets("t:MonoScript");

		// Figure out the "base" name if this is an open generic type definition
		var isOpenGeneric = type.IsGenericTypeDefinition; 
		var strippedBaseName = isOpenGeneric ? StripBacktick(type.Name) : null;

		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);

			// 1. If we see an open generic, fallback to checking filename
			if (isOpenGeneric && !string.IsNullOrEmpty(strippedBaseName))
			{
				var fileName = Path.GetFileName(path);
				if (fileName.Equals(strippedBaseName + ".cs", StringComparison.OrdinalIgnoreCase))
				{
					var fullPath = Application.dataPath.Replace("Assets", "") + path;
					if (File.Exists(fullPath))
					{
						return fullPath;
					}
				}
			}

			// 2. Otherwise, do the usual closed-generic / normal type check
			var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
			if (monoScript == null) continue;

			var scriptClass = monoScript.GetClass();
			if (scriptClass == null) continue;

			if (IsSameOrOpenGenericEquivalent(scriptClass, type))
			{
				var fullPath = Application.dataPath.Replace("Assets", "") + path;
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}
		}

		Debug.LogWarning($"Could not find a .cs file for type: {type.FullName}");
		return null;
	}

	private static bool IsSameOrOpenGenericEquivalent(Type scriptClass, Type targetType)
	{
		if (scriptClass.IsGenericType && targetType.IsGenericType)
		{
			return scriptClass.GetGenericTypeDefinition() == targetType.GetGenericTypeDefinition();
		}

		return scriptClass == targetType;
	}

	private static string StripBacktick(string typeName)
	{
		// e.g. "RoundListState`1" => "RoundListState"
		var index = typeName.IndexOf('`');
		return (index >= 0) 
			? typeName.Substring(0, index) 
			: typeName;
	}
}