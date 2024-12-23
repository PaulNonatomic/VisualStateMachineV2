using System.IO;

namespace Nonatomic.VSM2.Editor.Migration
{
#if UNITY_EDITOR
	using System;
	using UnityEditor;
	using UnityEngine;

	public static class MigrationUtils
	{
		public static string GetPathForType(Type type)
		{
			var guids = AssetDatabase.FindAssets("t:MonoScript");
		
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
				if (monoScript == null) continue;

				var scriptClass = monoScript.GetClass();
				if (scriptClass == null) continue;

				if (!IsSameOrOpenGenericEquivalent(scriptClass, type)) continue;

				var fullPath = Application.dataPath.Replace("Assets", "") + path;
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			Debug.LogWarning($"Could not find a .cs file for type: {type.FullName}");
			return null;
		}

		private static bool IsSameOrOpenGenericEquivalent(Type scriptClass, Type targetType)
		{
			// If both sides are generic, compare their definitions
			if (scriptClass.IsGenericType && targetType.IsGenericType)
			{
				return scriptClass.GetGenericTypeDefinition() == targetType.GetGenericTypeDefinition();
			}

			// Otherwise do a direct compare (for non-generic types)
			return scriptClass == targetType;
		}
	}
#endif

}