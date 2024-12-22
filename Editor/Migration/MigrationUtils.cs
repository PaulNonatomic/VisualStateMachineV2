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